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
        ///     Represents the service container used to 
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        ///     Creates a new instance of the <see cref="CommandService"/> for the target assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public CommandService(IServiceProvider services = null)
        {
            Services = services;
            CommandMap = new Dictionary<string, CommandInfo>();
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

            var method = type.GetMethod("ExecuteAsync");

            if (method is null)
                throw new InvalidOperationException($"An unexpected error has occurred while trying to find execution method of type {type.FullName}.");

            var ctor = type.GetConstructors().First();

            if (ctor is null)
                throw new InvalidOperationException($"An unexpected error has occurred while trying to find primary constructor of type {type.FullName}");

            var attr = type.GetCustomAttributes(false);

            foreach (var a in attr)
            {
                if (a is CommandAttribute cmd)
                {
                    var commandInfo = new CommandInfo(ctor, method, type, attr, cmd.Name, cmd.Description);

                    if (attr.Where(x => x is AliasesAttribute).FirstOrDefault() is AliasesAttribute ali)
                    {
                        string[] aliases = ali.Aliases;
                        foreach (var alias in aliases)
                        {
                            CommandMap.Add(alias, commandInfo);
                        }
                    }

                    CommandMap.Add(cmd.Name, commandInfo);
                }
                else
                    continue;
            }
        }

        /// <summary>
        ///     Executes the found command with the provided context.
        /// </summary>
        /// <param name="commandContext"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IResult> ExecuteCommandAsync<T>(T commandContext) where T : ICommandContext
        {
            if (CommandMap.TryGetValue(commandContext.Name, out var info))
            {
                var obj = info.Constructor.Invoke(null);

                if (obj is CommandBase<T> module)
                {

                    try
                    {
                        await ExecuteInternalAsync(commandContext, module, info);
                    }
                    catch (Exception ex)
                    {
                        return CommandResult.FromError(ex.Message, ex);
                    }

                    return CommandResult.FromSuccess();
                }
                else
                    return ModuleResult.FromError($"Failed to interpret module type with matching type of {nameof(CommandBase<T>)}");
            }

            return SearchResult.FromError($"Failed to find command with name: {commandContext.Name}");
        }

        private async Task ExecuteInternalAsync<T>(T context, CommandBase<T> module, CommandInfo info) where T : ICommandContext
        {
            module.SetContext(context);
            module.SetInformation(info);
            module.SetService(this);

            var stopwatch = Stopwatch.StartNew();

            await module.BeforeExecuteAsync(info, context);

            await module.ExecuteAsync();

            await module.AfterExecuteAsync(info, context);

            stopwatch.Stop();
        }
    }
}
