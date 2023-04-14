using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(true)]
namespace CSF
{
    /// <summary>
    ///     The root class of CSF, responsible for managing commands and their execution. 
    /// </summary>
    public class CommandManager
    {
        private readonly IServiceProvider _services;

        public IConditionalComponent[] Components { get; }

        public CommandManager(IServiceProvider serviceProvider)
        {
            _services = serviceProvider;

            var context = serviceProvider.GetRequiredService<CommandBuildingConfiguration>();

            Components = context.Build();
        }

        public virtual async ValueTask<CommandResult> ExecuteAsync(IContext context, CommandExecutionOptions options = null, CancellationToken cancellationToken = default)
        {
            async ValueTask<CommandResult> PipeAsync(IContext context, IServiceProvider services, CancellationToken cancellationToken)
            {
#if !DEBUG
                try
                {
#endif
                    var cell = Search(context, services);

                    var module = services.GetService(cell.Command.Module.Type) as ModuleBase;

                    module.Context = context;
                    module.Command = cell.Command;

                    await module.BeforeExecuteAsync(cancellationToken);

                    var value = cell.Command.Target.Invoke(module, cell.Arguments);

                    await module.AfterExecuteAsync(cancellationToken);

                    switch (value)
                    {
                        case Task task:
                            await task;
                            break;
                        case null:
                            break;
                        default:
                            throw new NotSupportedException("The return value of this command is not supported.");
                    }

                    return new CommandResult();
#if !DEBUG
                }
                catch (PipelineException ex)
                {
                    return ex.AsResult();
                }
                catch (Exception ex)
                {
                    return new ExecuteException(ex.Message, ex.InnerException).AsResult();
                }
#endif
            }

            options ??= CommandExecutionOptions.Default;

            var services = options.Scope?.ServiceProvider ?? _services;

            if (!options.ExecuteAsynchronously)
            {
                var result = await PipeAsync(context, services, cancellationToken);
                await AfterExecuteAsync(context, services, result);

                return result;
            }

            _ = Task.Run(async () =>
            {
                var result = await PipeAsync(context, services, cancellationToken);
                await AfterExecuteAsync(context, services, result);
            },
            cancellationToken);

            return new CommandResult();
        }

        protected virtual ValueTask AfterExecuteAsync(IContext context, IServiceProvider services, CommandResult result)
            => ValueTask.CompletedTask;

        public CommandCell Search(IContext context, IServiceProvider services)
        {
            var commands = Components.Search(context, services);

            if (commands.Length == 0)
                throw new SearchException("Failed to find any commands that accept the provided input.");

            foreach (var command in commands)
                if (!command.IsInvalid)
                    return command;

            throw new SearchException("Failed to find any commands that accept the provided input.", commands[0].Exception);
        }
    }
}
