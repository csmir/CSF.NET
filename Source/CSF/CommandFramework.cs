using CSF.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the handler for registered commands.
    /// </summary>
    public class CommandFramework
    {
        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public List<Command> CommandMap { get; private set; }

        /// <summary>
        ///     The configuration for this service.
        /// </summary>
        public CommandConfiguration Configuration { get; private set; }

        /// <summary>
        ///     The logger passed throughout the build and execution process.
        /// </summary>
        /// <remarks>
        ///     The resolver behind this logger is available for modification in <see cref="ConfigureLogger"/>.
        /// </remarks>
        public ILogger Logger { get; private set; }

        private readonly AsyncEvent<Func<IContext, IResult, Task>> _commandExecuted;
        /// <summary>
        ///     Invoked when a command is executed.
        /// </summary>
        /// <remarks>
        ///     This is the only way to do post-execution processing when <see cref="CommandConfiguration.DoAsynchronousExecution"/> is set to <see cref="true"/>.
        /// </remarks>
        public event Func<IContext, IResult, Task> CommandExecuted
        {
            add
                => _commandExecuted.Add(value);
            remove
                => _commandExecuted.Remove(value);
        }

        private readonly AsyncEvent<Func<Command, Task>> _commandRegistered;
        /// <summary>
        ///     Invoked when a command is registered.
        /// </summary>
        /// <remarks>
        ///     This event can be used to do additional registration steps for certain services.
        /// </remarks>
        public event Func<Command, Task> CommandRegistered
        {
            add
                => _commandRegistered.Add(value);
            remove
                => _commandRegistered.Remove(value);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework"/> with default configuration.
        /// </summary>
        public CommandFramework()
            : this(new CommandConfiguration())
        {

        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework"/> with provided configuration.
        /// </summary>
        /// <param name="config"></param>
        public CommandFramework(CommandConfiguration config)
        {
            CommandMap = new List<Command>();
            Configuration = config;

            Logger = ConfigureLogger();

            if (config.TypeReaders is null)
                config.TypeReaders = new TypeReaderDictionary(TypeReader.CreateDefaultReaders());

            _commandRegistered = new AsyncEvent<Func<Command, Task>>();
            _commandExecuted = new AsyncEvent<Func<IContext, IResult, Task>>();

            if (Configuration.AutoRegisterModules)
            {
                Logger.WriteDebug("Auto-registration enabled. Starting build process:");
                if (Configuration.RegistrationAssembly is null)
                {
                    Logger.WriteWarning("Auto-registration is enabled but the default registration assembly is not.");
                    return;
                }

                BuildModulesAsync(Configuration.RegistrationAssembly)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        /// <summary>
        ///     Configures the application logger and exposes it within the pipeline.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify what will be registered.
        /// </remarks>
        /// <returns></returns>
        protected virtual ILogger ConfigureLogger()
        {
#if RELEASE
            if (Configuration.DefaultLogLevel is LogLevel.Trace)
                Configuration.DefaultLogLevel = LogLevel.Debug;
#endif
            return new DefaultLogger(Configuration.DefaultLogLevel);
        }

        /// <summary>
        ///     Registers all command modules in the provided assembly to the <see cref="CommandMap"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the registration flow.
        /// </remarks>
        /// <param name="assembly">The assembly to find all modules for.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual async Task BuildModulesAsync(Assembly assembly)
        {
            if (assembly is null)
            {
                Logger.WriteError("Expected a not-null value.", new ArgumentNullException(nameof(assembly)));
                return;
            }

            var types = assembly.GetTypes();

            var baseType = typeof(IModuleBase);

            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    Logger.WriteTrace($"Found module by name: {type.Name}.");
                    await BuildModuleAsync(type);
                }
            }
        }

        /// <summary>
        ///     Registers a single module to the <see cref="CommandMap"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the registration flow.
        /// </remarks>
        /// <param name="type">The <see cref="ModuleBase{T}"/> to register.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual async Task BuildModuleAsync(Type type)
        {
            if (type is null)
            {
                Logger.WriteError("Expected a not-null value.", new ArgumentNullException(nameof(type)));
                return;
            }

            var module = new Module(type);

            var methods = type.GetMethods();

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(true);

                string name = null;
                string[] aliases = Array.Empty<string>();
                foreach (var attribute in attributes)
                {
                    if (attribute is CommandAttribute commandAttribute)
                    {
                        if (string.IsNullOrEmpty(commandAttribute.Name))
                        {
                            Logger.WriteWarning("Expected a not-null and not-empty command name.", new ArgumentNullException(method.Name));
                            continue;
                        }
                        name = commandAttribute.Name;
                        Logger.WriteTrace($"Found command by name: {name}");
                    }

                    if (attribute is AliasesAttribute aliasesAttribute)
                        aliases = aliasesAttribute.Aliases;
                }

                if (string.IsNullOrEmpty(name))
                    continue;

                aliases = new[] { name }.Concat(aliases).ToArray();
                Logger.WriteTrace($"Concatenated aliases for {name}: {string.Join(", ", aliases)}");

                try
                {
                    var command = new Command(Configuration, module, method, aliases);

                    if (Configuration.InvokeOnlyNameRegistrations)
                    {
                        if (!CommandMap.Any(x => x.Aliases.SequenceEqual(command.Aliases)))
                        {
                            await _commandRegistered.InvokeAsync(command);
                            Logger.WriteTrace($"Invoked registration event for {command.Name}");
                        }
                    }
                    else
                    {
                        await _commandRegistered.InvokeAsync(command);
                        Logger.WriteTrace($"Invoked registration event for {command.Name}");
                    }

                    CommandMap.Add(command);
                    Logger.WriteDebug($"Registered command {command.Name}.");
                }
                catch (Exception ex)
                {
                    Logger.WriteCritical(ex.Message, ex);
                    return;
                }
            }
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
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public async Task<IResult> ExecuteCommandAsync<T>(T context, IServiceProvider provider = null)
            where T : IContext
        {
            if (provider is null)
                provider = EmptyServiceProvider.Instance;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, provider);
                    await _commandExecuted.InvokeAsync(context, result);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, provider);
                await _commandExecuted.InvokeAsync(context, result);
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
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        protected virtual async Task<IResult> RunPipelineAsync<T>(T context, IServiceProvider provider)
            where T : IContext
        {
            Logger.WriteDebug($"Starting command pipeline for name: '{context.Name}'");

            var searchResult = await SearchAsync(context);
            if (!searchResult.IsSuccess)
                return searchResult;

            var command = searchResult.Match;

            var preconResult = await CheckAsync(context, command, provider);
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
                commandBase: (ModuleBase<T>)constructResult.Result,
                parameters: ((ParseResult)readResult).Result.ToArray());
        }

        /// <summary>
        ///     Searches through the <see cref="CommandMap"/> for the best possible match.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns></returns>
        protected virtual async Task<SearchResult> SearchAsync<T>(T context)
            where T : IContext
        {
            await Task.CompletedTask;

            var commands = CommandMap
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
                return CommandNotFoundResult(context);

            Command match = null;
            foreach (var command in commands)
            {
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
                if (commandLength < contextLength)
                {
                    foreach (var parameter in command.Parameters)
                    {
                        if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                            match = command;
                    }
                }

                // If context length is lower than command length, return the command with least optionals.
                if (commandLength > contextLength)
                {
                    int requiredLength = 1;
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
                return NoApplicableOverloadResult(context);

            Logger.WriteTrace($"Found matching command for name: '{context.Name}': {match.Name}");

            return SearchResult.FromSuccess(match);
        }

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult CommandNotFoundResult<T>(T context)
            where T : IContext
        {
            return SearchResult.FromError($"Failed to find command with name: {context.Name}.");
        }

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult NoApplicableOverloadResult<T>(T context)
            where T : IContext
        {
            return SearchResult.FromError($"Failed to find overload that best matches input: {context.RawInput}.");
        }

        /// <summary>
        ///     Runs all preconditions found on the command and its module and returns the result.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how checks are done and what should be returned if certain checks fail.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="PreconditionResult"/> of the first failed check or success if all checks succeeded.</returns>
        protected virtual async Task<PreconditionResult> CheckAsync<T>(T context, Command command, IServiceProvider provider)
            where T : IContext
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
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ConstructionResult"/> of the built module.</returns>
        protected virtual async Task<ConstructionResult> ConstructAsync<T>(T context, Command command, IServiceProvider provider)
            where T : IContext
        {
            await Task.CompletedTask;

            var services = new List<object>();
            foreach (var dependency in command.Module.Constructor.Dependencies)
            {
                if (dependency.Type is IServiceProvider)
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

            if (!(obj is ModuleBase<T> commandBase))
                return InvalidModuleTypeResult(context, command.Module);

            commandBase.SetContext(context);
            commandBase.SetInformation(command);

            commandBase.SetLogger(Logger);
            commandBase.SetSource(this);

            Logger.WriteTrace($"Succesfully constructed module for {command.Name}");

            return ConstructionResult.FromSuccess(commandBase);
        }

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult ServiceNotFoundResult<T>(T context, Dependency dependency)
            where T : IContext
        {
            return ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");
        }

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="IModuleBase"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="IModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult InvalidModuleTypeResult<T>(T context, Module module)
            where T : IContext
        {
            return ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<T>)}");
        }

        /// <summary>
        ///     Tries to parse all parameter inputs provided by the <see cref="IContext"/> to its expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to parameter parsing or catch certain attributes being present.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ParseResult"/> of the parsed parameters of this command.</returns>
        protected virtual async Task<IResult> ParseAsync<T>(T context, Command command, IServiceProvider provider)
            where T : IContext
        {
            int index = 0;
            var parameters = new List<object>();

            foreach (var param in command.Parameters)
            {
                if (param.Flags.HasFlag(ParameterFlags.IsRemainder))
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (param.Flags.HasFlag(ParameterFlags.IsOptional) && context.Parameters.Count <= index)
                {
                    var missingResult = await ResolveMissingValue(param);

                    if (!missingResult.IsSuccess)
                        break;

                    var resultType = missingResult.Result.GetType();
                    if (resultType != param.Type)
                        return ParseResult.FromError($"Returned type does not match expected result. Expected: '{param.Type.Name}'. Got: '{resultType.Name}'");

                    else
                        parameters.Add(missingResult.Result);
                }

                if (param.Type == typeof(string) || param.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                var result = await param.TypeReader.ReadAsync(context, param, context.Parameters[index], provider);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            Logger.WriteTrace($"Succesfully populated parameters for {command.Name}");

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        /// <summary>
        ///     Called when the first optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add uses for <see cref="SelfIfNullAttribute"/>'s.
        ///     <br/>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="Parameter.Type"/>.
        /// </remarks>
        /// <param name="param"></param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> for the target parameter.</returns>
        protected virtual Task<TypeReaderResult> ResolveMissingValue(Parameter param)
        {
            return Task.FromResult(TypeReaderResult.FromError("Unresolved."));
        }

        /// <summary>
        ///     Tries to execute the provided command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to the execution of the command with all prior steps completed.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="commandBase">The module needed to execute the command method.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="parameters">The parsed parameters required to populate the command method.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ExecuteResult"/> of this executed command.</returns>
        protected virtual async Task<IResult> ExecuteAsync<T>(T context, Command command, ModuleBase<T> commandBase, object[] parameters)
            where T : IContext
        {
            try
            {
                Logger.WriteDebug($"Starting execution of {commandBase.CommandInfo.Name}.");
                var sw = Stopwatch.StartNew();

                await commandBase.BeforeExecuteAsync(command, context);

                var result = command.Method.Invoke(commandBase, parameters.ToArray());

                switch (result)
                {
                    case Task<IResult> execTask:
                        var asyncResult = await execTask;
                        if (!asyncResult.IsSuccess)
                            return asyncResult;
                        break;
                    case Task task:
                        await task;
                        break;
                    case ExecuteResult syncResult:
                        if (!syncResult.IsSuccess)
                            return syncResult;
                        break;
                    default:
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
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>A <see cref="ExecuteResult"/> holding the returned error.</returns>
        protected virtual ExecuteResult UnhandledExceptionResult<T>(T context, Command command, Exception ex)
        {
            return ExecuteResult.FromError(ex.Message, ex);
        }
    }
}
