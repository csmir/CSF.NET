namespace CSF
{
    public partial class CommandManager
    {
        public virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, CommandExecutionOptions options = null)
        {
            options ??= CommandExecutionOptions.Default;

            var services = options.Scope?.ServiceProvider ?? _services;

            if (!options.ExecuteAsynchronously)
            {
                var result = await ExecuteAsync(context, services);
                await AfterExecuteAsync(context, services, result);

                return result;
            }

            _ = Task.Run(async () =>
            {
                var result = await ExecuteAsync(context, services);
                await AfterExecuteAsync(context, services, result);
            });

            return new();
        }

        public virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services)
        {
            try
            {
                var cell = Search(context, services);

                return await ExecuteAsync(context, services, cell);
            }
            catch (PipelineException ex)
            {
                return ex.AsResult();
            }
            catch (Exception ex)
            {
                return new ExecuteException(ex.Message, ex).AsResult();
            }
        }

        public virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, CommandCell cell)
        {
            var value = cell.Execute(context, services);

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
        public static object Execute(this CommandCell cell, ICommandContext context, IServiceProvider services)
        {
            var module = services.GetService(cell.Command.Module.Type) as ModuleBase;

            module.Context = context;
            module.Command = cell.Command;

            var value = cell.Command.Target.Invoke(module, cell.Arguments);

            return value;
        }
    }
}
