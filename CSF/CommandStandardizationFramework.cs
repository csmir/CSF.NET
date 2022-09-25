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
    public class CommandStandardizationFramework 
    {
        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public List<CommandInfo> CommandMap { get; private set; }

        /// <summary>
        ///     The range of registered typereaders.
        /// </summary>
        public Dictionary<Type, ITypeReader> TypeReaders { get; private set; }

        /// <summary>
        ///     The configuration for this service.
        /// </summary>
        public CommandConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Invoked when a command is registered.
        /// </summary>
        public event Func<CommandInfo, Task> CommandRegistered;

        /// <summary>
        ///     Invoked when a command is executed.
        /// </summary>
        /// <remarks>
        ///     This is the only way to do post-execution processing when <see cref="CommandConfiguration.DoAsynchronousExecution"/> is set to <see cref="true"/>.
        /// </remarks>
        public event Func<ICommandContext, IResult, ICommandBase, Task> CommandExecuted;

        /// <summary>
        ///     Creates a new instance of <see cref="CommandStandardizationFramework"/> with default configuration.
        /// </summary>
        public CommandStandardizationFramework()
            : this(new CommandConfiguration())
        {

        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandStandardizationFramework"/> with provided configuration.
        /// </summary>
        /// <param name="config"></param>
        public CommandStandardizationFramework(CommandConfiguration config)
        {
            CommandMap = new List<CommandInfo>();
            TypeReaders = BaseTypeReader.RegisterAll();
            Configuration = config;
        }

        /// <summary>
        ///     Registers all command modules in the provided assembly to the <see cref="CommandMap"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the registration flow.
        /// </remarks>
        /// <param name="assembly">The assembly to find all modules for.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual async Task RegisterModulesAsync(Assembly assembly)
        {
            var types = assembly.GetTypes();

            var baseType = typeof(ICommandBase);

            foreach (var type in types)
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                    await RegisterModuleAsync(type);
        }

        /// <summary>
        ///     Registers a single module to the <see cref="CommandMap"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the registration flow.
        /// </remarks>
        /// <param name="type">The <see cref="CommandBase{T}"/> to register.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual async Task RegisterModuleAsync(Type type)
        {
            await Task.CompletedTask;

            var module = new ModuleInfo(type);

            var methods = type.GetMethods();

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(true);

                string name = null;
                string[] aliases = Array.Empty<string>();
                foreach (var attribute in attributes)
                {
                    if (attribute is CommandAttribute commandAttribute)
                        name = commandAttribute.Name;

                    if (attribute is AliasesAttribute aliasesAttribute)
                        aliases = aliasesAttribute.Aliases;
                }

                if (string.IsNullOrEmpty(name))
                    break;

                aliases = new[] { name }.Concat(aliases).ToArray();

                var command = new CommandInfo(TypeReaders, module, method, aliases);

                CommandMap.Add(command);
            }
        }

        /// <summary>
        ///     Adds an <see cref="ITypeReader"/> to the framework.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify registration steps.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="replaceExisting"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual bool RegisterTypeReader<T>(ITypeReader reader, bool replaceExisting = true)
        {
            var type = typeof(T);

            if (!(reader is TypeReader<T> realReader))
                throw new InvalidOperationException($"This {nameof(ITypeReader)} is not supported for {type.FullName}.");

            if (TypeReaders.ContainsKey(type)) 
            {
                if (replaceExisting)
                {
                    TypeReaders[type] = realReader;
                    return true;
                }
                return false;
            }

            TypeReaders.Add(type, realReader);
            return true;
        }

        /// <summary>
        ///     Removes a typereader from the list of existing type readers.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify removal steps.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool RemoveTypeReader<T>()
            => TypeReaders.Remove(typeof(T));

        /// <summary>
        ///     Tries to execute a command with provided <see cref="ICommandContext"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="CommandConfiguration.DoAsynchronousExecution"/> is enabled, the <see cref="IResult"/> of this method will always return success.
        ///     Use the <see cref="CommandExecuted"/> event to do post-execution processing.
        ///     <br/><br/>
        ///     This method can be overridden to modify the execution entry flow. 
        ///     If you want to change the order of execution or add extra steps, override <see cref="RunPipelineAsync{T}(T, IServiceProvider)"/> instead.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public async Task<IResult> ExecuteCommandAsync<T>(T context, IServiceProvider provider = null) 
            where T : ICommandContext
        {
            if (provider is null)
                provider = EmptyServiceProvider.Instance;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, provider);
                    // invoke command executed event
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, provider);
                // invoke command executed event
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
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        protected virtual async Task<IResult> RunPipelineAsync<T>(T context, IServiceProvider provider) 
            where T : ICommandContext
        {
            // search input for command
            var searchResult = await SearchAsync(context);

            if (!searchResult.IsSuccess)
                return searchResult;

            var command = searchResult.Match;

            // check preconditions
            var preconResult = await CheckAsync(context, command, provider);

            if (!preconResult.IsSuccess)
                return preconResult;

            // create class object & inject services
            var constructResult = await ConstructAsync(context, command, provider);

            if (!constructResult.IsSuccess)
                return constructResult;

            var commandBase = (CommandBase<T>)constructResult.Result;

            // read typereaders & populate command
            var readResult = await ParseAsync(context, command, provider);

            if (!readResult.IsSuccess)
                return readResult;

            var parameters = ((ParseResult)readResult).Result;

            // run command
            return await ExecuteAsync(context, commandBase, command, parameters.ToArray());
        }

        /// <summary>
        ///     Searches through the <see cref="CommandMap"/> for the best possible match.
        /// </summary>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <returns></returns>
        protected virtual async Task<SearchResult> SearchAsync<T>(T context)
            where T : ICommandContext
        {
            await Task.CompletedTask;

            var commands = CommandMap
                .Where(command => command.Aliases.Any(alias => string.Equals(alias == context.Name, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
                return CommandNotFoundResult(context);

            CommandInfo match = null;
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
                        if (parameter.IsRemainder)
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

                        if (!parameter.IsOptional)
                        {
                            requiredLength++;
                        }
                    }
                }
            }

            if (match is null)
                return NoApplicableOverloadResult(context);

            return SearchResult.FromSuccess(match);
        }

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult CommandNotFoundResult<T>(T context)
            where T : ICommandContext
        {
            return SearchResult.FromError($"Failed to find command with name: {context.Name}.");
        }

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult NoApplicableOverloadResult<T>(T context)
            where T : ICommandContext
        {
            return SearchResult.FromError($"Failed to find overload that best matches input: {context.RawInput}.");
        }

        /// <summary>
        ///     Runs all preconditions found on the command and its module and returns the result.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how checks are done and what should be returned if certain checks fail.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="PreconditionResult"/> of the first failed check or success if all checks succeeded.</returns>
        protected virtual async Task<PreconditionResult> CheckAsync<T>(T context, CommandInfo command, IServiceProvider provider)
            where T : ICommandContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider);

                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        /// <summary>
        ///     Creates a new instance of the module the command needs to execute and injects its services.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how the command module is injected and constructed.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ConstructionResult"/> of the built module.</returns>
        protected virtual async Task<ConstructionResult> ConstructAsync<T>(T context, CommandInfo command, IServiceProvider provider)
            where T : ICommandContext
        {
            await Task.CompletedTask;

            var services = new List<object>();
            foreach (var service in command.Module.ServiceTypes)
            {
                if (service.ServiceType is IServiceProvider)
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(service.ServiceType);

                    if (t is null && !service.IsNullable)
                        return ServiceNotFoundResult(context, service);

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.Invoke(services.ToArray());

            if (!(obj is CommandBase<T> commandBase))
                return InvalidModuleTypeResult(context, command.Module);

            commandBase.SetContext(context);
            commandBase.SetInformation(command);
            commandBase.SetService(this);

            return ConstructionResult.FromSuccess(commandBase);
        }

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="service">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult ServiceNotFoundResult<T>(T context, ServiceInfo service)
            where T : ICommandContext
        {
            return ConstructionResult.FromError($"The service of type {service.ServiceType.FullName} does not exist in the current {nameof(IServiceProvider)}.");
        }

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="ICommandBase"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="ICommandBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult InvalidModuleTypeResult<T>(T context, ModuleInfo module)
            where T : ICommandContext
        {
            return ConstructionResult.FromError($"Failed to interpret module of type {module.ModuleType.FullName} with type of {nameof(CommandBase<T>)}");
        }

        /// <summary>
        ///     Tries to parse all parameter inputs provided by the <see cref="ICommandContext"/> to its expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to parameter parsing or catch certain attributes being present.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ParseResult"/> of the parsed parameters of this command.</returns>
        protected virtual async Task<IResult> ParseAsync<T>(T context, CommandInfo command, IServiceProvider provider)
            where T : ICommandContext
        {
            int index = 0;
            var parameters = new List<object>();

            foreach (var param in command.Parameters)
            {
                if (param.IsRemainder)
                {
                    parameters.Add(string.Join(" ", context.Parameters.GetRange(index, context.Parameters.Count - (index - 1))));
                    break;
                }

                if (param.IsOptional && context.Parameters.Count <= index)
                    break;

                var result = await param.Reader.ReadAsync(context, param, context.Parameters[index], provider);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        /// <summary>
        ///     Tries to execute the provided command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to the execution of the command with all prior steps completed.
        /// </remarks>
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="commandBase">The module needed to execute the command method.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="parameters">The parsed parameters required to populate the command method.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ExecuteResult"/> of this executed command.</returns>
        protected virtual async Task<ExecuteResult> ExecuteAsync<T>(T context, CommandBase<T> commandBase, CommandInfo command, object[] parameters)
            where T : ICommandContext
        {
            try
            {
                await commandBase.BeforeExecuteAsync(command, context);

                var result = command.Method.Invoke(commandBase, parameters.ToArray());

                if (result is Task task)
                    await task;

                if (result is Task<ExecuteResult> resultTask)
                {
                    var executeResult = await resultTask;

                    return executeResult;
                }

                await commandBase.AfterExecuteAsync(command, context);

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
        /// <typeparam name="T">The <see cref="ICommandContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="ICommandContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>A <see cref="ExecuteResult"/> holding the returned error.</returns>
        protected virtual ExecuteResult UnhandledExceptionResult<T>(T context, CommandInfo command, Exception ex)
        {
            return ExecuteResult.FromError(ex.Message, ex);
        }
    }
}
