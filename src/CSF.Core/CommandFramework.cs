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
        ///         <item><description>Defines <paramref name="pipelineService"/> as the specified <see cref="IPipelineService"/> for this builder.</description></item>
        ///         <item><description>Creates a new <see cref="CommandConfiguration"/> with default values.</description></item>
        ///         <item><description>Sets up a new <see cref="IServiceProvider"/> from <see cref="EmptyServiceProvider.Instance"/> with no defined services.</description></item>
        ///         <item><description>Sets up a new <see cref="IHandlerBuilder"/> exposed in <see cref="IFrameworkBuilder.HandlerBuilder"/> for result handling.</description></item>
        ///     </list>
        /// </remarks>
        /// <param name="pipelineService">The pipeline service.</param>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with default arguments and the defined <see cref="IPipelineService"/>.</returns>
        public static IFrameworkBuilder CreateDefaultBuilder<TService>(TService pipelineService)
            where TService : PipelineService
        {
            return new FrameworkBuilder<TService>(pipelineService);
        }

        /// <summary>
        ///     Creates a new <see cref="IFrameworkBuilder"/> with default setup.
        /// </summary>
        /// <remarks>
        ///     This method sets up a number of default values for the returned <see cref="IFrameworkBuilder"/>.
        ///     <list type="bullet">
        ///         <item><description>Creates a new <see cref="IPipelineService"/> with unmodified arguments.</description></item>
        ///         <item><description>Creates a new <see cref="CommandConfiguration"/> with default values.</description></item>
        ///         <item><description>Creates a new <see cref="IServiceProvider"/> from <see cref="EmptyServiceProvider.Instance"/> with no defined services.</description></item>
        ///         <item><description>Creates a new <see cref="IHandlerBuilder"/> exposed in <see cref="IFrameworkBuilder.HandlerBuilder"/> for default result handling.</description></item>
        ///     </list>
        /// </remarks>
        /// <returns>A new <see cref="IFrameworkBuilder"/> with default arguments.</returns>
        public static IFrameworkBuilder CreateDefaultBuilder()
        {
            return new FrameworkBuilder<PipelineService>(new PipelineService());
        }

        /// <summary>
        ///     Creates a new <see cref="ICommandFramework"/> with default setup directly. It skips all configuration.
        /// </summary>
        /// <remarks>
        ///     This method sets up the command framework with configuration that matches <see cref="CreateDefaultBuilder"/>. Read this method for more information.
        /// </remarks>
        /// <returns>A new <see cref="ICommandFramework"/>.</returns>
        public static ICommandFramework BuildWithMinimalSetup()
        {
            return CreateDefaultBuilder()
                .Build();
        }
    }

    /// <summary>
    ///     Represents the handler for registering and handling incoming commands.
    /// </summary>
    public sealed class CommandFramework<T> : ICommandFramework
        where T : PipelineService
    {
        /// <summary>
        ///     The pipeline provider that handles command creation, handling & result control.
        /// </summary>
        public T PipelineService { get; }

        /// <inheritdoc/>
        public IList<IConditionalComponent> Commands { get; }

        /// <inheritdoc/>
        public TypeReaderProvider TypeReaders { get; }

        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; }

        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(T pipelineProvider)
            : this(new CommandConfiguration(), pipelineProvider)
        {
            
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(CommandConfiguration configuration, T pipelineProvider)
            : this(EmptyServiceProvider.Instance, configuration, pipelineProvider)
        {
            
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/>.
        /// </summary>
        public CommandFramework(IServiceProvider serviceProvider, CommandConfiguration configuration, T pipelineProvider)
        {
            configuration.Prefixes ??= new PrefixProvider();
            configuration.TypeReaders ??= new TypeReaderProvider();
            
            Configuration = configuration;

            PipelineService = pipelineProvider;
            Services = serviceProvider;
        }

        /// <inheritdoc/>
        public async Task RunAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default)
        {
            if (autoConfigureAssemblies)
            {
                if (Configuration.RegistrationAssemblies is null || !Configuration.RegistrationAssemblies.Any())
                    throw new MissingValueException("Array was found to be null or empty.", nameof(Configuration.RegistrationAssemblies));

                await ConfigureTypeReadersAsync(cancellationToken);

                await ConfigureModulesAsync(cancellationToken);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                var input = await PipelineService.GetInputAsync(cancellationToken);

                var context = await PipelineService.BuildContextAsync(input, cancellationToken);

                await ExecuteCommandsAsync(context, cancellationToken: cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async ValueTask ConfigureModulesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var assembly in Configuration.RegistrationAssemblies)
                await BuildModulesAsync(assembly, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async ValueTask BuildModulesAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            var mt = typeof(IModuleBase);

            foreach (var type in assembly.ExportedTypes)
                if (mt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    await BuildModuleAsync(type, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async ValueTask BuildModuleAsync(Type type, CancellationToken cancellationToken)
        {
            var module = ModuleInfo.Build(Configuration.TypeReaders, type);

            foreach (var component in module.Components)
            {
                Commands.Add(component);
                await component.RequestToHandleAsync(PipelineService, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async ValueTask ConfigureTypeReadersAsync(CancellationToken cancellationToken = default)
        {
            foreach (var assembly in Configuration.RegistrationAssemblies)
                await BuildTypeReadersAsync(assembly, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async ValueTask BuildTypeReadersAsync(Assembly assembly, CancellationToken cancellationToken)
        {
            var tt = typeof(ITypeReader);

            foreach (var type in assembly.ExportedTypes)
                if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    await BuildTypeReaderAsync(type, cancellationToken);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async ValueTask BuildTypeReaderAsync(Type type, CancellationToken cancellationToken)
        {
            var reader = TypeReaderInfo.Build(type);

            var output = reader.Construct(Services);

            if (output is null)
                throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (output is ITypeReader typeReader)
            {
                Configuration.TypeReaders.Include(typeReader);
                await typeReader.RequestToHandleAsync(PipelineService, cancellationToken).ConfigureAwait(false);
            }

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(ITypeReader)}.");
        }

        /// <inheritdoc/>
        public bool TryParsePrefix(ref string rawInput, out IPrefix prefix)
        {
            if (Configuration.Prefixes.TryGetPrefix(rawInput, out prefix))
            {
                rawInput = rawInput[prefix.Value.Length..].TrimStart();
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public async Task<IResult> ExecuteCommandsAsync<TContext>(TContext context, IServiceProvider services = null, CancellationToken cancellationToken = default)
            where TContext : IContext
        {
            services ??= Services;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, services, cancellationToken).ConfigureAwait(false);

                    await result.RequestToHandleAsync(context, PipelineService, cancellationToken).ConfigureAwait(false);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, services, cancellationToken).ConfigureAwait(false);

                await result.RequestToHandleAsync(context, PipelineService, cancellationToken).ConfigureAwait(false);

                return result;
            }
        }

        private async ValueTask<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            Logger.Debug($"Starting command pipeline for name: '{context.Name}'");

            // find commands
            var searchResult = await SearchAsync(context).ConfigureAwait(false);

            if (!searchResult.IsSuccess)
                return searchResult;

            // check commands for availability.
            var checkResult = await CheckAsync(context, searchResult.Result, provider, cancellationToken).ConfigureAwait(false);

            if (!checkResult.IsSuccess)
                return checkResult;

            // select commands.
            var commands = checkResult.Result;
            if (!Configuration.ExecuteAllValidMatches)
                commands = new[] { checkResult.Result[0] };

            foreach (var command in commands)
            {
                // build module.
                var constructResult = await ConstructAsync(context, command, provider).ConfigureAwait(false);

                if (!constructResult.IsSuccess)
                    return constructResult;

                // parse types.
                var readResult = await ParseAsync(context, command, cancellationToken).ConfigureAwait(false);

                if (!readResult.IsSuccess)
                    return readResult;

                // execute command.
                var result = await ExecuteAsync(context, command, (ModuleBase<TContext>)constructResult.Result, ((ParseResult)readResult).Result, cancellationToken).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;
            }
            return ExecuteResult.FromSuccess();
        }

        private async ValueTask<SearchResult> SearchAsync<TContext>(TContext context)
            where TContext : IContext
        {
            var matches = Commands
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.CastWhere<ModuleInfo>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.CastWhere<CommandInfo>()
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

                    var result = await SearchModuleAsync(context, groups[0]).ConfigureAwait(false);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return await SearchCommandsAsync(context, commands).ConfigureAwait(false);

            return PipelineService.OnCommandNotFound(context);
        }

        private async ValueTask<SearchResult> SearchModuleAsync<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
        {
            var matches = module.Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.CastWhere<CommandInfo>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.CastWhere<ModuleInfo>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return await SearchModuleAsync(context, groups[0]).ConfigureAwait(false);
                }

                else
                    return PipelineService.OnCommandNotFound(context);
            }

            return await SearchCommandsAsync(context, commands).ConfigureAwait(false);
        }

        private ValueTask<SearchResult> SearchCommandsAsync<TContext>(TContext context, IEnumerable<CommandInfo> commands)
            where TContext : IContext
        {
            CommandInfo overload = null;
            IEnumerable<CommandInfo> SearchMatches()
            {
                foreach (var command in commands)
                {
                    if (overload is null && command.IsErrorOverload)
                        overload = command;

                    var commandLength = command.Parameters.Count;
                    var contextLength = context.Parameters.Count;

                    // If parameter & input length is equal, prefer it over all matches.
                    if (commandLength == contextLength)
                        yield return command;

                    // If command length is lower than context length, look for a remainder attribute.
                    // Due to sorting upwards, it will continue the loop and prefer the remainder attr with most parameters.
                    if (commandLength <= contextLength)
                        foreach (var parameter in command.Parameters)
                            if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                                yield return command;

                    // If context length is lower than command length, return the command with least optionals.
                    if (commandLength > contextLength)
                    {
                        int requiredLength = 0;
                        foreach (var parameter in command.Parameters)
                            if (!parameter.Flags.HasFlag(ParameterFlags.IsOptional))
                                requiredLength++;

                        if (contextLength >= requiredLength)
                            yield return command;
                    }
                }
            }

            var matches = SearchMatches();

            if (!matches.Any())
            {
                if (overload is null)
                    return PipelineService.OnBestOverloadUnavailable(context);
                else
                    return SearchResult.FromSuccess(new[] { overload });
            }

            Logger.Trace($"Found matches for name: '{context.Name}': {string.Join(", ", matches)}");

            return SearchResult.FromSuccess(matches.ToArray());
        }

        private async ValueTask<CheckResult> CheckAsync<TContext>(TContext context, IEnumerable<CommandInfo> matches, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            var commands = new List<CommandInfo>();
            var failureResult = PreconditionResult.FromSuccess();

            foreach (var match in matches)
            {
                var result = await CheckPreconditionsAsync(context, match, provider, cancellationToken);

                if (result.IsSuccess)
                    commands.Add(match);
                else
                    failureResult = result;
            }

            if (!commands.Any())
                return CheckResult.FromError(failureResult.ErrorMessage, failureResult.Exception);

            return CheckResult.FromSuccess(commands.ToArray());
        }

        private async ValueTask<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider, cancellationToken).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;
            }

            Logger.Trace($"Succesfully ran precondition checks for {command.Name}.");

            return PreconditionResult.FromSuccess();
        }

        private ValueTask<ConstructionResult> ConstructAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider)
            where TContext : IContext
        {
            var services = new List<object>();
            foreach (var dependency in command.Module.Constructor.Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return PipelineService.OnServiceNotFound(context, dependency);

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.EntryPoint.Invoke(services.ToArray());

            if (obj is ModuleBase<TContext> commandBase)
            {
                commandBase.SetContext(context);
                commandBase.SetInformation(command);

                commandBase.SetLogger(Logger);

                Logger.Trace($"Succesfully constructed module for {command.Name}");

                return ConstructionResult.FromSuccess(commandBase);
            }
            return PipelineService.OnInvalidModule(context, command.Module);
        }

        private async ValueTask<IResult> ParseAsync<TContext>(TContext context, CommandInfo command, CancellationToken cancellationToken)
            where TContext : IContext
        {
            int index = 0;
            var parameters = new List<object>();

            foreach (var parameter in command.Parameters)
            {
                if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (parameter.Flags.HasFlag(ParameterFlags.IsOptional) && context.Parameters.Count <= index)
                {
                    var missingResult = PipelineService.OnMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return PipelineService.OnOptionalNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return PipelineService.OnMissingReturnedInvalid(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                var result = await parameter.TypeReader.ReadAsync(context, parameter, context.Parameters[index], cancellationToken).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            Logger.Trace($"Succesfully populated parameters for {command.Name}. Count: {parameters.Count}");

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        private async ValueTask<IResult> ExecuteAsync<TContext>(TContext context, CommandInfo command, ModuleBase<TContext> commandBase, object[] parameters, CancellationToken cancellationToken)
            where TContext : IContext
        {
            try
            {
                Logger.Debug($"Starting execution of {commandBase.CommandInfo.Name}.");
                var sw = Stopwatch.StartNew();

                await commandBase.BeforeExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

                var returnValue = command.Method.Invoke(commandBase, parameters);

                switch (returnValue)
                {
                    case Task<IResult> execTask:
                        var asyncResult = await execTask;
                        if (!asyncResult.IsSuccess)
                            return asyncResult;
                        break;
                    case Task task:
                        await task;
                        break;
                    case IResult syncResult:
                        if (!syncResult.IsSuccess)
                            return syncResult;
                        break;
                    default:
                        if (returnValue is null)
                            break;
                        var returnResult = PipelineService.OnUnhandledReturnType(context, returnValue);
                        if (!returnResult.IsSuccess)
                            return returnResult;
                        break;
                }

                await commandBase.AfterExecuteAsync(command, context, cancellationToken).ConfigureAwait(false);

                sw.Stop();
                Logger.Debug($"Finished execution of {commandBase.CommandInfo.Name} in {sw.ElapsedMilliseconds} ms.");

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return PipelineService.OnUnhandledException(context, command, ex);
            }
        }
    }
}
