using CSF.Results.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace CSF
{
    /// <summary>
    ///     Represents the handler for registered commands.
    /// </summary>
    public static class CommandFramework
    {
        /// <summary>
        ///     Creates a new <see cref="IFrameworkBuilder"/> from the provided pipeline setup.
        /// </summary>
        /// <remarks>
        ///     This method sets up a number of default values for the returned <see cref="IFrameworkBuilder"/>.
        ///     <list type="bullet">
        ///         <item><description>Defines <paramref name="conveyor"/> as the specified <see cref="ICommandConveyor"/> for this builder.</description></item>
        ///         <item><description>Creates a new <see cref="CommandConfiguration"/> with default values.</description></item>
        ///         <item><description>Sets up a new <see cref="IServiceProvider"/> from <see cref="EmptyServiceProvider.Instance"/> with no defined services.</description></item>
        ///     </list>
        /// </remarks>
        /// <param name="conveyor">The pipeline service.</param>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with default arguments and the defined <see cref="ICommandConveyor"/>.</returns>
        public static IFrameworkBuilder CreateDefaultBuilder<T>(T conveyor)
            where T : CommandConveyor
            => new FrameworkBuilder<T>(conveyor);

        /// <summary>
        ///     Creates a new <see cref="IFrameworkBuilder"/> with default setup.
        /// </summary>
        /// <remarks>
        ///     This method sets up a number of default values for the returned <see cref="IFrameworkBuilder"/>.
        ///     <list type="bullet">
        ///         <item><description>Creates a new <see cref="ICommandConveyor"/> with unmodified arguments.</description></item>
        ///         <item><description>Creates a new <see cref="CommandConfiguration"/> with default values.</description></item>
        ///         <item><description>Creates a new <see cref="IServiceProvider"/> from <see cref="EmptyServiceProvider.Instance"/> with no defined services.</description></item>
        ///     </list>
        /// </remarks>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with default arguments.</returns>
        public static IFrameworkBuilder CreateDefaultBuilder()
            => new FrameworkBuilder<CommandConveyor>(new CommandConveyor());

        /// <summary>
        ///     Creates a new <see cref="ICommandFramework"/> with default setup directly. It skips all configuration.
        /// </summary>
        /// <remarks>
        ///     This method sets up the command framework with configuration that matches <see cref="CreateDefaultBuilder"/>. Read this method for more information.
        /// </remarks>
        /// <returns>A new <see cref="ICommandFramework"/>.</returns>
        public static ICommandFramework BuildWithMinimalSetup()
            => CreateDefaultBuilder()
                .Build();
    }

    /// <summary>
    ///     Represents the handler for registering and handling incoming commands.
    /// </summary>
    /// <remarks>
    ///     This is the root type of CSF. This type is responsible for:
    ///     <list type="bullet">
    ///         <item><description>Resolving all defined <see cref="ModuleBase{T}"/>'s and registering their commands.</description></item>
    ///         <item><description>Handling inbound commands and resolving their pipeline steps.</description></item>
    ///         <item><description>Holding references and making calls to the defined <see cref="ICommandConveyor"/>.</description></item>
    ///         <item><description>Logging the workflow to the provided <see cref="ILogger"/>.</description></item>
    ///     </list>
    /// </remarks>
    /// <typeparam name="T">The conveyor used to provide support in handling the command pipeline.</typeparam>
    public class CommandFramework<T> : ICommandFramework
        where T : CommandConveyor
    {
        private CancellationTokenSource _globalSource;

        /// <summary>
        ///     The conveyor that handles command creation, handle results and result invocation.
        /// </summary>
        public T Conveyor { get; }

        /// <inheritdoc/>
        public IList<IConditionalComponent> Commands { get; }

        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; }

        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(T conveyor)
            : this(new CommandConfiguration(), conveyor)
        {

        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(CommandConfiguration configuration, T conveyor)
            : this(EmptyServiceProvider.Instance, configuration, conveyor)
        {

        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(IServiceProvider serviceProvider, CommandConfiguration configuration, T conveyor)
        {
            configuration.TypeReaders ??= new TypeReaderProvider();

            Configuration = configuration;

            Conveyor = conveyor;
            Services = serviceProvider;

            Commands = new List<IConditionalComponent>();

            Conveyor.SetLogger(Configuration, Services);
        }

        /// <inheritdoc/>
        public virtual async Task StartAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            _ = RunAsync(autoConfigureAssemblies, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task RunAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default)
        {
            _globalSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (autoConfigureAssemblies)
            {
                if (Configuration.RegistrationAssemblies is null || !Configuration.RegistrationAssemblies.Any())
                    throw new MissingValueException("Array was found to be null or empty.", nameof(Configuration.RegistrationAssemblies));

                await ConfigureTypeReadersAsync(_globalSource.Token);

                await ConfigureModulesAsync(_globalSource.Token);
            }

            await RunAsync();
        }

        /// <summary>
        ///     internally starts a loop that listens for commands and executes them.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual async Task RunAsync()
        {
            try
            {
                while (!_globalSource.IsCancellationRequested)
                {
                    var input = await Conveyor.GetInputAsync(_globalSource.Token);

                    var parseResult = Configuration.Parser.Parse(input);

                    if (!parseResult.IsSuccess)
                    {
                        Conveyor.Logger.Send(Conveyor.OnInvalidPrefix().AsLog());
                        continue;
                    }

                    var context = await Conveyor.BuildContextAsync(parseResult, _globalSource.Token);

                    await ExecuteCommandsAsync(context, cancellationToken: _globalSource.Token);
                }
            }
            catch
            {
                _globalSource.Cancel();
                throw;
            }
        }

        /// <inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            if (_globalSource is null)
                throw new MemberUnpreparedException();

            _globalSource.Cancel();
        }

        /// <inheritdoc/>
        public virtual async ValueTask ConfigureModulesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var assembly in Configuration.RegistrationAssemblies)
                await BuildModulesAsync(assembly, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildModulesAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            var mt = typeof(ModuleBase);

            foreach (var type in assembly.ExportedTypes)
                if (mt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    await BuildModuleAsync(type, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildModuleAsync(Type type, CancellationToken cancellationToken)
        {
            var module = Module.Build(Configuration.TypeReaders, type);

            foreach (var component in module.Components)
            {
                Commands.Add(component);
                await component.RequestToHandleAsync(Conveyor, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask ConfigureTypeReadersAsync(CancellationToken cancellationToken = default)
        {
            foreach (var assembly in Configuration.RegistrationAssemblies)
                await BuildTypeReadersAsync(assembly, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildTypeReadersAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            var tt = typeof(ITypeReader);

            foreach (var type in assembly.ExportedTypes)
                if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    await BuildTypeReaderAsync(type, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual async ValueTask BuildTypeReaderAsync(Type type, CancellationToken cancellationToken)
        {
            var reader = Reader.Build(type);

            IEnumerable<object> GetServices()
            {
                foreach (var dependency in reader.Constructor.Dependencies)
                {
                    if (dependency.Type == typeof(IServiceProvider))
                        yield return Services;
                    if (dependency.Type == typeof(ICommandFramework))
                        yield return this;
                    else
                    {
                        var t = Services.GetService(dependency.Type);

                        if (t is null && dependency.Flags.HasFlag(ParameterFlags.Nullable))
                            yield return Type.Missing;
                        else if (t is null)
                            throw new ArgumentNullException(nameof(t), $"Service of type {dependency.Type.Name} is not available in the {nameof(IServiceProvider)}.");

                        yield return t;
                    }
                }
            }

            var obj = reader.Constructor.EntryPoint.Invoke(GetServices().ToArray()) 
                ?? throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (obj is ITypeReader typeReader)
            {
                Configuration.TypeReaders.Include(typeReader);
                await typeReader.RequestToHandleAsync(Conveyor, cancellationToken);
            }

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(ITypeReader)}.");
        }

        /// <inheritdoc/>
        public virtual async Task<IResult> ExecuteCommandsAsync<TContext>(TContext context, IServiceProvider services = null, CancellationToken cancellationToken = default)
            where TContext : IContext
        {
            services ??= Services;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, services, cancellationToken);

                    await result.RequestToHandleAsync(context, Conveyor, cancellationToken);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, services, cancellationToken);

                await result.RequestToHandleAsync(context, Conveyor, cancellationToken);

                return result;
            }
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            Conveyor.Logger.Debug($"Starting command pipeline for name: '{context.Name}'");

            var searchResult = await SearchAsync(context, cancellationToken);

            if (!searchResult.IsSuccess)
                return searchResult;

            var checkResult = await CheckAsync(context, searchResult.Result, provider, cancellationToken);

            if (!checkResult.IsSuccess)
                return checkResult;

            var (command, args) = ((CheckYieldResult)checkResult).Result.First();

            var constructResult = await ConstructAsync(context, command, provider, cancellationToken);

            if (!constructResult.IsSuccess)
                return constructResult;

            var result = await ExecuteAsync(context, command, constructResult.Result, args.Result, cancellationToken);

            if (result != null && !result.IsSuccess)
                return result;

            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<SearchResult> SearchAsync<TContext>(TContext context, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var matches = Commands
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.CastWhere<Module>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.CastWhere<Command>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (groups.Any())
            {
                if (context.Parameters.Count > 0)
                {
                    var oldName = context.Name;
                    var oldParam = context.Parameters;

                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    var result = await SearchModuleAsync(context, groups[0], cancellationToken);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return SearchResult.FromSuccess(commands);

            return Conveyor.OnCommandNotFound(context);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<SearchResult> SearchModuleAsync<TContext>(TContext context, Module module, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var matches = module.Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.CastWhere<Command>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.CastWhere<Module>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return await SearchModuleAsync(context, groups[0], cancellationToken);
                }

                else
                    return Conveyor.OnCommandNotFound(context);
            }

            return SearchResult.FromSuccess(commands);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<IResult> CheckAsync<TContext>(TContext context, IEnumerable<Command> commands, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            IResult failureResult = null;

            var matchResult = await CheckMatchesAsync(context, commands, cancellationToken);

            if (!matchResult.IsSuccess)
                return matchResult;

            commands = matchResult.Result;

            var matches = new List<(Command, ArgsResult)>();

            foreach (var command in commands)
            {
                var preconResult = await CheckPreconditionsAsync(context, command, provider, cancellationToken);

                if (preconResult.IsSuccess)
                {
                    var readResult = await ReadAsync(context, command, cancellationToken);

                    if (!readResult.IsSuccess)
                        failureResult = readResult;
                    else
                        matches.Add((command, (ArgsResult)readResult));
                }
                else
                    failureResult ??= preconResult;
            }

            if (!matches.Any())
                return CheckResult.FromError(failureResult.ErrorMessage, failureResult.Exception);

            return CheckYieldResult.FromSuccess(matches);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual ValueTask<CheckResult> CheckMatchesAsync<TContext>(TContext context, IEnumerable<Command> commands, CancellationToken cancellationToken)
            where TContext : IContext
        {
            Command overload = null;
            IEnumerable<Command> SearchMatches()
            {
                foreach (var command in commands)
                {
                    if (overload is null && command.IsErrorOverload)
                        overload = command;

                    var min = command.MaxLength;
                    var contextLength = context.Parameters.Count;

                    // If parameter & input length is equal, prefer it over all matches.
                    if (command.MaxLength == contextLength)
                        yield return command;

                    // If command length is lower than context length, look for a remainder attribute.
                    // Due to sorting upwards, it will continue the loop and prefer the remainder attr with most parameters.
                    if (command.MaxLength <= contextLength)
                        foreach (var parameter in command.Parameters)
                            if (parameter.Flags.HasFlag(ParameterFlags.Remainder))
                                yield return command;

                    // If context length is lower than command length, return the command with least optionals.
                    if (command.MaxLength > contextLength)
                    {
                        if (command.MinLength <= contextLength)
                            yield return command;
                    }
                }
            }

            var matches = SearchMatches();

            if (!matches.Any())
            {
                if (overload is null)
                    return Conveyor.OnBestOverloadUnavailable(context);
                else
                    return CheckResult.FromSuccess(new[] { overload });
            }

            Conveyor.Logger.Trace($"Found matches for name: '{context.Name}': {string.Join(", ", matches)}");

            return CheckResult.FromSuccess(matches);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider, cancellationToken);

                if (!result.IsSuccess)
                    return result;
            }

            Conveyor.Logger.Trace($"Succesfully ran precondition checks for {command.Name}.");

            return PreconditionResult.FromSuccess();
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual ValueTask<ConstructionResult> ConstructAsync<TContext>(TContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            IEnumerable<object> GetServices()
            {
                foreach (var dependency in command.Module.Constructor.Dependencies)
                {
                    if (dependency.Type == typeof(IServiceProvider))
                        yield return provider;
                    if (dependency.Type == typeof(ICommandFramework))
                        yield return this;
                    else
                    {
                        var t = provider.GetService(dependency.Type);

                        if (t is null && !dependency.Flags.HasFlag(ParameterFlags.Nullable))
                            yield return Conveyor.OnServiceNotFound(context, dependency);

                        yield return t;
                    }
                }
            }

            var obj = command.Module.Constructor.EntryPoint.Invoke(GetServices().ToArray());

            if (obj is ModuleBase commandBase)
            {
                commandBase.SetContext(context);
                commandBase.SetInformation(command);

                commandBase.SetLogger(Conveyor.Logger);

                Conveyor.Logger.Trace($"Succesfully constructed module for {command.Name}");

                return ConstructionResult.FromSuccess(commandBase);
            }
            return Conveyor.OnInvalidModule(context, command.Module);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<IResult> ReadAsync<TContext>(TContext context, Command command, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var result = await ReadContainerAsync(context, 0, command, cancellationToken);

            if (!result.IsSuccess)
                return result;

            return ArgsResult.FromSuccess(((ArgsResult)result).Result, -1);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<IResult> ReadContainerAsync<TContext>(TContext context, int index, IParameterContainer container, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var parameters = new List<object>();

            foreach (var parameter in container.Parameters)
            {
                if (parameter.Flags.HasRemainder())
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (parameter.Flags.HasOptional() && context.Parameters.Count <= index)
                {
                    var missingResult = Conveyor.OnMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return Conveyor.OnOptionalNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return Conveyor.OnMissingReturnedInvalid(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                if (parameter.Flags.HasNullable() && context.Parameters[index] is string str && (str == "null" || str == "nothing"))
                {
                    parameters.Add(null);
                    index++;
                    continue;
                }

                if (parameter is ComplexParameter complexParam)
                {
                    var result = await ReadContainerAsync(context, index, complexParam, cancellationToken);

                    if (!result.IsSuccess)
                        return result;

                    if (result is ArgsResult argsResult)
                    {
                        index = argsResult.Placement;
                        parameters.Add(complexParam.Constructor.EntryPoint.Invoke(argsResult.Result.ToArray()));
                    }
                }

                else if (parameter is BaseParameter normal)
                {
                    var result = await normal.TypeReader.ReadAsync(context, normal, context.Parameters[index], cancellationToken);

                    if (!result.IsSuccess)
                        return result;

                    index++;
                    parameters.Add(result.Result);
                }
            }

            Conveyor.Logger.Trace($"Succesfully populated parameters. Count: {parameters.Count}");

            return ArgsResult.FromSuccess(parameters, index);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual async ValueTask<IResult> ExecuteAsync<TContext>(TContext context, Command command, ModuleBase module, IEnumerable<object> parameters, CancellationToken cancellationToken)
            where TContext : IContext
        {
            try
            {
                Conveyor.Logger.Debug($"Starting execution of {module.CommandInfo.Name}.");
                var sw = Stopwatch.StartNew();

                await module.BeforeExecuteAsync(command, cancellationToken);

                var returnValue = command.Method.Invoke(module, parameters.ToArray());

                await module.AfterExecuteAsync(command, cancellationToken);

                sw.Stop();
                Conveyor.Logger.Debug($"Finished execution of {module.CommandInfo.Name} in {sw.ElapsedMilliseconds} ms.");

                var result = default(IResult);

                switch (returnValue)
                {
                    case Task<IResult> execTask:
                        result = await execTask;
                        break;
                    case Task task:
                        await task;
                        result = ExecuteResult.FromSuccess();
                        break;
                    case IResult syncResult:
                        if (!syncResult.IsSuccess)
                            return syncResult;
                        break;
                    default:
                        if (returnValue is null)
                            break;
                        result = Conveyor.OnUnhandledReturnType(context, returnValue);
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                return Conveyor.OnUnhandledException(context, command, ex);
            }
        }
    }
}
