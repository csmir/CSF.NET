namespace CSF
{
    public partial class CommandManager
    {
        public virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, CommandExecutionOptions options = null, CancellationToken cancellationToken = default)
        {
            options ??= CommandExecutionOptions.Default;

            var services = options.Scope?.ServiceProvider ?? _services;

            if (!options.ExecuteAsynchronously)
            {
                var result = await ExecuteAsync(context, services, cancellationToken);
                await AfterExecuteAsync(context, services, result);

                return result;
            }

            _ = Task.Run(async () =>
            {
                var result = await ExecuteAsync(context, services, cancellationToken);
                await AfterExecuteAsync(context, services, result);
            },
            cancellationToken);

            return new();
        }

        public virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken)
        {
#if !DEBUG
            try
            {
#endif
                var cell = Search(context, services);

                return await ExecuteAsync(context, services, cell, cancellationToken);
#if !DEBUG
            }
            catch (PipelineException ex)
            {
                return ex.AsResult();
            }
            catch (Exception ex)
            {
                return new ExecuteException(ex.Message, ex).AsResult();
            }
#endif
        }

        public virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, CommandCell cell, CancellationToken cancellationToken)
        {
            var value = await cell.ExecuteAsync(context, services, cancellationToken);

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
        }

        protected virtual ValueTask AfterExecuteAsync(ICommandContext context, IServiceProvider services, CommandResult result)
            => ValueTask.CompletedTask;
    }

    internal static class ExecuteOperations
    {
        public static async ValueTask<object> ExecuteAsync(this CommandCell cell, ICommandContext context, IServiceProvider services, CancellationToken cancellationToken)
        {
            var module = services.GetService(cell.Command.Module.Type) as ModuleBase;

            module.Context = context;
            module.Command = cell.Command;

            await module.BeforeExecuteAsync(cancellationToken);

            var value = cell.Command.Target.Invoke(module, cell.Arguments);

            await module.AfterExecuteAsync(cancellationToken);

            return value;
        }
    }
}
