using CSF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace CSF
{
    /// <summary>
    ///     Represents the handler for registered commands.
    /// </summary>
    public partial class CommandFramework<T>
        where T : Configurator
    {
        /// <summary>
        ///     The configurator that handles command creation.
        /// </summary>
        public T Configurator { get; }

        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public IList<IConditionalComponent> Commands { get; }

        private TypeReaderProvider _typeReaders;
        /// <summary>
        ///     The range of registered typereaders.
        /// </summary>
        public TypeReaderProvider TypeReaders
        {
            get
            {
                if (_typeReaders is null)
                    _typeReaders = Configurator.ConfigureTypeReaders();
                return _typeReaders;
            }
        }

        private PrefixProvider _prefixes;
        /// <summary>
        ///     The range of registered prefixes.
        /// </summary>
        public PrefixProvider Prefixes
        {
            get
            {
                if (_prefixes is null)
                    _prefixes = Configurator.ConfigurePrefixes();
                return _prefixes;
            }
        }

        private ResultHandlerProvider _resultHandlers;
        /// <summary>
        ///     The range of registered result handlers.
        /// </summary>
        public ResultHandlerProvider ResultHandlers
        {
            get
            {
                if (_resultHandlers is null)
                    _resultHandlers = Configurator.ConfigureResultHandlers();
                return _resultHandlers;
            }
        }

        private ILogger _logger;
        /// <summary>
        ///     The logger passed throughout the build and execution process.
        /// </summary>
        /// <remarks>
        ///     The resolver behind this logger is available for modification in <see cref="ConfigureLogger"/>.
        /// </remarks>
        public ILogger Logger
        {
            get
            {
                if (_logger is null)
                    _logger = Configurator.ConfigureLogger();
                return _logger;
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework"/> with provided configuration.
        /// </summary>
        /// <param name="config"></param>
        public CommandFramework(T configurator)
        {
            Configurator = configurator;
            Commands = new List<IConditionalComponent>();
        }

        public void BuildModuleAssemblies()
        {
            foreach (var assembly in Configurator.Configuration.RegistrationAssemblies)
            {
                BuildModuleAssembly(assembly);
            }
        }

        public void BuildModuleAssembly(Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetEntryAssembly();

            var mt = typeof(IModuleBase);

            foreach (var type in assembly.ExportedTypes)
            {
                if (mt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                {
                    BuildModule(type);
                    return;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void BuildModule(Type type)
        {
            var module = ModuleInfo.Build(TypeReaders, type);

            foreach (var component in module.Components)
                Commands.Add(component);
        }

        /// <summary>
        ///     Tries to parse an <see cref="IPrefix"/> from the provided raw input and will remove the length of the prefix from it.
        /// </summary>
        /// <remarks>
        ///     This method will browse the <see cref="PrefixProvider"/> from the <see cref="Configuration"/> of this instance.
        /// </remarks>
        /// <param name="rawInput">The raw text input to try and parse a prefix for.</param>
        /// <param name="prefix">The resulting prefix. <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a matching <see cref="IPrefix"/> was found in the <see cref="PrefixProvider"/>. <see langword="false"/> if not.</returns>
        public bool TryParsePrefix(ref string rawInput, out IPrefix prefix)
        {
            if (Prefixes.TryGetPrefix(rawInput, out prefix))
            {
                rawInput = rawInput.Substring(prefix.Value.Length).TrimStart();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Tries to execute a command with provided <see cref="IContext"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="CommandConfiguration.DoAsynchronousExecution"/> is enabled, the <see cref="IResult"/> of this method will always return success.
        ///     Use the <see cref="CommandExecuted"/> event to do post-execution processing.
        ///     <br/><br/>
        ///     If you want to change the order of execution or add extra steps, override <see cref="RunPipelineAsync{T}(T, IServiceProvider)"/>.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public async Task<IResult> ExecuteCommandAsync<TContext>(TContext context, IServiceProvider provider = null)
            where TContext : IContext
        {
            provider = provider
                ?? Configurator.Provider
                ?? EmptyServiceProvider.Instance;

            if (Configurator.Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, provider);

                    await ResultHandlers.InvokeHandlers(context, result);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, provider);

                await ResultHandlers.InvokeHandlers(context, result);

                return result;
            }
        }

        /// <summary>
        ///     Runs all execution steps and executes the targetted command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the pipeline order. 
        ///     Not providing all existing steps while overriding this method will cause unpredictable behavior.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        protected virtual async Task<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider)
            where TContext : IContext
        {
            Logger.WriteDebug($"Starting command pipeline for name: '{context.Name}'");

            var searchResult = await SearchAsync(context);
            if (!searchResult.IsSuccess)
                return searchResult;

            var command = searchResult.Match;

            var preconResult = await CheckPreconditionsAsync(context, command, provider);
            if (!preconResult.IsSuccess)
                return preconResult;

            var constructResult = await ConstructAsync(context, command, provider);
            if (!constructResult.IsSuccess)
                return constructResult;

            var readResult = await ParseAsync(context, command, provider);
            if (!readResult.IsSuccess)
                return readResult;

            return await ExecuteAsync(
                context: context,
                command: command,
                commandBase: (ModuleBase<TContext>)constructResult.Result,
                parameters: ((ParseResult)readResult).Result.ToArray());
        }

        /// <summary>
        ///     Searches through the <see cref="Commands"/> for the best possible match.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to change the search flow.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding a <see cref="SearchResult"/> with the returned result.</returns>
        protected virtual async Task<SearchResult> SearchAsync<TContext>(TContext context)
            where TContext : IContext
        {
            await Task.CompletedTask;

            var matches = Commands
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.SelectWhere<ModuleInfo>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.SelectWhere<CommandInfo>()
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

                    var result = await SearchModuleAsync(context, groups[0]);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return await SearchCommandsAsync(context, commands);

            return CommandNotFoundResult(context);
        }

        /// <summary>
        ///     Searches a module for the best fitting command.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to change the search flow.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module to search in.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding a <see cref="SearchResult"/> with the returned result.</returns>
        protected virtual async Task<SearchResult> SearchModuleAsync<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
        {
            var matches = module.Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.SelectWhere<CommandInfo>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.SelectWhere<ModuleInfo>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return await SearchModuleAsync(context, groups[0]);
                }

                else
                    return CommandNotFoundResult(context);
            }

            return await SearchCommandsAsync(context, commands);
        }

        /// <summary>
        ///     Searches a range of commands in the provided group for the best match.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to change the search flow.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="commands">The commands to seek through.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding a <see cref="SearchResult"/> with the returned result.</returns>
        protected virtual async Task<SearchResult> SearchCommandsAsync<TContext>(TContext context, IEnumerable<CommandInfo> commands)
            where TContext : IContext
        {
            await Task.CompletedTask;

            CommandInfo match = null;
            CommandInfo errormatch = null;
            foreach (var command in commands)
            {
                if (errormatch is null && command.IsErrorOverload)
                    errormatch = command;

                var commandLength = command.Parameters.Count;
                var contextLength = context.Parameters.Count;

                // If parameter & input length is equal, prefer it over all matches & exit the loop.
                if (commandLength == contextLength)
                {
                    match = command;
                    break;
                }

                // If command length is lower than context length, look for a remainder attribute.
                // Due to sorting upwards, it will continue the loop and prefer the remainder attr with most parameters.
                if (commandLength <= contextLength)
                {
                    foreach (var parameter in command.Parameters)
                    {
                        if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                        {
                            match = command;
                            break;
                        }
                    }
                }

                // If context length is lower than command length, return the command with least optionals.
                if (commandLength > contextLength)
                {
                    int requiredLength = 0;
                    foreach (var parameter in command.Parameters)
                    {
                        if (contextLength >= requiredLength)
                        {
                            match = command;
                            break;
                        }

                        if (!parameter.Flags.HasFlag(ParameterFlags.IsOptional))
                        {
                            requiredLength++;
                        }
                    }
                }
            }

            if (match is null)
            {
                if (errormatch is null)
                    return NoApplicableOverloadResult(context);
                else
                    return SearchResult.FromSuccess(errormatch);
            }

            Logger.WriteTrace($"Found matching command for name: '{context.Name}': {match.Name}");

            return SearchResult.FromSuccess(match);
        }

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult CommandNotFoundResult<TContext>(TContext context)
            where TContext : IContext
        {
            return SearchResult.FromError($"Failed to find command with name: {context.Name}.");
        }

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult NoApplicableOverloadResult<TContext>(TContext context)
            where TContext : IContext
        {
            return SearchResult.FromError($"Failed to find overload that best matches input: {context.Name}.");
        }

        /// <summary>
        ///     Runs all preconditions found on the command and its module and returns the result.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how checks are done and what should be returned if certain checks fail.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="PreconditionResult"/> of the first failed check or success if all checks succeeded.</returns>
        protected virtual async Task<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider)
            where TContext : IContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider);

                if (!result.IsSuccess)
                    return result;
            }

            Logger.WriteTrace($"Succesfully ran precondition checks for {command.Name}.");

            return PreconditionResult.FromSuccess();
        }

        /// <summary>
        ///     Creates a new instance of the module the command needs to execute and injects its services.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how the command module is injected and constructed.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ConstructionResult"/> of the built module.</returns>
        protected virtual async Task<ConstructionResult> ConstructAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider)
            where TContext : IContext
        {
            await Task.CompletedTask;

            var services = new List<object>();
            foreach (var dependency in command.Module.Constructor.Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return ServiceNotFoundResult(context, dependency);

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.EntryPoint.Invoke(services.ToArray());

            if (!(obj is ModuleBase<TContext> commandBase))
                return InvalidModuleTypeResult(context, command.Module);

            commandBase.SetContext(context);
            commandBase.SetInformation(command);

            commandBase.SetLogger(Logger);

            Logger.WriteTrace($"Succesfully constructed module for {command.Name}");

            return ConstructionResult.FromSuccess(commandBase);
        }

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult ServiceNotFoundResult<TContext>(TContext context, DependencyInfo dependency)
            where TContext : IContext
        {
            return ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");
        }

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="IModuleBase"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="IModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult InvalidModuleTypeResult<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
        {
            return ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<TContext>)}");
        }

        /// <summary>
        ///     Tries to parse all parameter inputs provided by the <see cref="IContext"/> to its expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to parameter parsing or catch certain attributes being present.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ParseResult"/> of the parsed parameters of this command.</returns>
        protected virtual async Task<IResult> ParseAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider)
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
                    var missingResult = await ResolveMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return OptionalValueNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return MissingOptionalFailedMatch(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                var result = await parameter.TypeReader.ReadAsync(context, parameter, context.Parameters[index]);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            Logger.WriteTrace($"Succesfully populated parameters for {command.Name}. Count: {parameters.Count}");

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        /// <summary>
        ///     Called when an optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add uses for <see cref="SelfIfNullAttribute"/>'s.
        ///     <br/>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="ParameterInfo.Type"/> or <see cref="Type.Missing"/>.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> for the target parameter.</returns>
        protected virtual Task<TypeReaderResult> ResolveMissingValue<TContext>(TContext context, ParameterInfo param)
            where TContext : IContext
        {
            return Task.FromResult(TypeReaderResult.FromSuccess(Type.Missing));
        }

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, ParameterInfo)"/> returned a type that did not match the expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="expectedType">The type that was expected to return.</param>
        /// <param name="returnedType">The returned type.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        protected virtual ParseResult MissingOptionalFailedMatch<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext
        {
            return ParseResult.FromError($"Returned type does not match expected result. Expected: '{expectedType.Name}'. Got: '{returnedType.Name}'");
        }

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, ParameterInfo)"/> failed to return a valid result. 
        ///     This method has to return <see cref="Type.Missing"/> if no self-implemented value has been returned.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        protected virtual ParseResult OptionalValueNotPopulated<TContext>(TContext context)
            where TContext : IContext
        {
            return ParseResult.FromError($"Optional parameter did not get {nameof(Type.Missing)} or self-populated value.");
        }

        /// <summary>
        ///     Tries to execute the provided command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to the execution of the command with all prior steps completed.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="commandBase">The module needed to execute the command method.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="parameters">The parsed parameters required to populate the command method.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ExecuteResult"/> of this executed command.</returns>
        protected virtual async Task<IResult> ExecuteAsync<TContext>(TContext context, CommandInfo command, ModuleBase<TContext> commandBase, object[] parameters)
            where TContext : IContext
        {
            try
            {
                Logger.WriteDebug($"Starting execution of {commandBase.CommandInfo.Name}.");
                var sw = Stopwatch.StartNew();

                await commandBase.BeforeExecuteAsync(command, context);

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

                        var unhandledResult = ProcessUnhandledReturnType(context, returnValue);
                        if (!unhandledResult.IsSuccess)
                            return unhandledResult;

                        break;
                }

                await commandBase.AfterExecuteAsync(command, context);

                sw.Stop();
                Logger.WriteDebug($"Finished execution of {commandBase.CommandInfo.Name} in {sw.ElapsedMilliseconds} ms.");

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return UnhandledExceptionResult(context, command, ex);
            }
        }

        /// <summary>
        ///     Returns the error message when an unhandled return type has been returned from the command method.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="returnValue">The returned value of the method.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        protected virtual ExecuteResult ProcessUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext
        {
            return ExecuteResult.FromError($"Received an unhandled type from method execution: {returnValue.GetType().Name}. \n\rConsider overloading {nameof(ProcessUnhandledReturnType)} if this is intended.");
        }

        /// <summary>
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        protected virtual ExecuteResult UnhandledExceptionResult<TContext>(TContext context, CommandInfo command, Exception ex)
            where TContext : IContext
        {
            return ExecuteResult.FromError(ex.Message, ex);
        }
    }
}
