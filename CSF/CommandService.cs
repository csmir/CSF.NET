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
    public class CommandService
    {
        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public Dictionary<string, CommandInfo> CommandMap { get; private set; }

        /// <summary>
        ///     The range of registered typereaders.
        /// </summary>
        public Dictionary<Type, ITypeReader> TypeReaders { get; private set; }

        /// <summary>
        ///     Creates a new instance of the <see cref="CommandService"/> for the target assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public CommandService()
        {
            CommandMap = new Dictionary<string, CommandInfo>();
            TypeReaders = new Dictionary<Type, ITypeReader>();
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

                var command = new CommandInfo(TypeReaders, module, method, name);

                CommandMap.Add(name, command);

                foreach (var alias in aliases)
                {
                    CommandMap.Add(alias, command);
                }
            }
        }

        /// <summary>
        ///     Adds a <see cref="ITypeReader"/> to the framework.
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

        public bool RemoveTypeReader<T>()
            => TypeReaders.Remove(typeof(T));

        /// <summary>
        ///     Executes the found command with the provided context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IResult> ExecuteCommandAsync<T>(T context, IServiceProvider provider = null) where T : ICommandContext
        {
            if (provider is null)
                provider = EmptyServiceProvider.Instance;

            if (CommandMap.TryGetValue(context.Name, out var command))
            {
                var services = new List<object>();
                foreach (var param in command.Module.ServiceTypes)
                {
                    if (param.Type is IServiceProvider)
                        services.Add(provider);
                    else
                    {
                        var t = provider.GetService(param.Type);
                        services.Add(t);
                    }
                }

                var obj = command.Module.Constructor.Invoke(services.ToArray());

                if (obj is CommandBase<T> module)
                {
                    int index = 0;
                    var parameters = new List<object>();

                    foreach (var param in command.Parameters)
                    {
                        if (!param.IsOptional && context.Parameters.Count <= index)
                            return TypeReaderResult.FromError("Not enough parameters have been provided.");

                        if (param.IsOptional && context.Parameters.Count <= index)
                            break;

                        var result = await param.Reader.ReadAsync(context, param, context.Parameters[index], provider);

                        if (!result.IsSuccess)
                            return result;
                        parameters.Add(result.Result);
                        index++;
                    }

                    foreach (var precon in command.Preconditions)
                    {
                        var result = await precon.CheckAsync(context, command, provider);

                        if (!result.IsSuccess)
                            return result;
                    }

                    try
                    {
                        module.SetContext(context);
                        module.SetInformation(command);
                        module.SetService(this);

                        var stopwatch = Stopwatch.StartNew();

                        await module.BeforeExecuteAsync(command, context);

                        var result = command.Method.Invoke(module, parameters.ToArray());

                        if (result is Task t)
                            await t;

                        if (result is Task<ExecuteResult> ct)
                        {
                            var executeResult = await ct;

                            if (!executeResult.IsSuccess)
                                return executeResult;
                        }

                        await module.AfterExecuteAsync(command, context);

                        stopwatch.Stop();

                        return ExecuteResult.FromSuccess();
                    }
                    catch (Exception ex)
                    {
                        return ExecuteResult.FromError(ex.Message, ex);
                    }
                }
                else
                    return ModuleResult.FromError($"Failed to interpret module type with matching type of {nameof(CommandBase<T>)}");
            }

            return SearchResult.FromError($"Failed to find command with name: {context.Name}");
        }
    }
}
