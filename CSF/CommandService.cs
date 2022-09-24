using CSF.Commands;
using CSF.Info;
using CSF.Results;
using CSF.TypeReaders;
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
            TypeReaders = ValueTypeReader.RegisterAll();
            Configuration = config;
        }

        /// <summary>
        ///     Registers all commands in the provided assembly to the <see cref="CommandMap"/> for execution.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public async Task RegisterCommandsAsync(Assembly assembly)
        {
            var types = assembly.GetTypes();

            var baseType = typeof(ICommandBase);

            foreach (var type in types)
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract)
                    await RegisterInternalAsync(type);

            //if (Configuration.UseRegistrationTools)
            //    foreach (var command in CommandMap)
            //        await HandleRegisterTasksAsync(command);
        }

        private async Task RegisterInternalAsync(Type type)
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
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="replaceExisting"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool RegisterTypeReader<T>(ITypeReader reader, bool replaceExisting = true)
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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool RemoveTypeReader<T>()
            => TypeReaders.Remove(typeof(T));

        /// <summary>
        ///     Executes the found command with the provided context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IResult> ExecuteCommandAsync<T>(T context, IServiceProvider provider = null) 
            where T : ICommandContext
        {
            if (provider is null)
                provider = EmptyServiceProvider.Instance;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = HandlePipelineAsync(context, provider);
                return ExecuteResult.FromSuccess();
            }

            else
                return await HandlePipelineAsync(context, provider);
        }

        private async Task<IResult> HandlePipelineAsync<T>(T context, IServiceProvider provider) 
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

        private async Task<SearchResult> SearchAsync<T>(T context)
            where T : ICommandContext
        {
            await Task.CompletedTask;

            var commands = CommandMap
                .Where(command => command.Aliases.Any(alias => string.Equals(alias == context.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (commands.Count < 1)
                return SearchResult.FromError($"Failed to find command with name: {context.Name}");

            if (commands.Count > 1)
            {
                var bestResult = commands[0];
                var paramCount = context.Parameters.Count;

                foreach (var command in commands)
                {
                    if (command.Parameters.Count == paramCount)
                        bestResult = command;
                }

                return SearchResult.FromSuccess(bestResult);
            }

            return SearchResult.FromSuccess(commands[0]);
        }

        private async Task<PreconditionResult> CheckAsync<T>(T context, CommandInfo command, IServiceProvider provider)
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

        private async Task<ConstructionResult> ConstructAsync<T>(T context, CommandInfo command, IServiceProvider provider)
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
                        return ConstructionResult.FromError($"The service of type {service.ServiceType.FullName} does not exist in the current {nameof(IServiceProvider)}.");

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.Invoke(services.ToArray());

            if (!(obj is CommandBase<T> commandBase))
                return ConstructionResult.FromError($"Failed to interpret module type with matching type of {nameof(CommandBase<T>)}");

            commandBase.SetContext(context);
            commandBase.SetInformation(command);
            commandBase.SetService(this);

            return ConstructionResult.FromSuccess(commandBase);
        }

        private async Task<IResult> ParseAsync<T>(T context, CommandInfo command, IServiceProvider provider)
            where T : ICommandContext
        {
            int index = 0;
            var parameters = new List<object>();

            foreach (var param in command.Parameters)
            {
                if (!param.IsOptional && context.Parameters.Count <= index)
                    return ParseResult.FromError("Not enough parameters have been provided.");

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

        private async Task<ExecuteResult> ExecuteAsync<T>(T context, CommandBase<T> commandBase, CommandInfo command, object[] parameters)
            where T : ICommandContext
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                await commandBase.BeforeExecuteAsync(command, context);

                var result = command.Method.Invoke(commandBase, parameters.ToArray());

                if (result is Task t)
                    await t;

                if (result is Task<ExecuteResult> ct)
                {
                    var executeResult = await ct;

                    if (!executeResult.IsSuccess)
                        return executeResult;
                }

                await commandBase.AfterExecuteAsync(command, context);

                stopwatch.Stop();

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex.Message, ex);
            }
        }
    }
}
