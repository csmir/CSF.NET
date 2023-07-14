namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Executes a command based on the input provided through the <see cref="ICommandContext"/> and options passed into this method.
        /// </summary>
        /// <remarks>
        ///     The <see cref="CommandResult"/> returned from this method changes at the discretion of <see cref="CommandExecutionOptions.ExecuteAsynchronously"/>, as passed in when the method is called. 
        ///     To guarantee the command result to be handled under all circumstances, 
        ///     consider overriding <see cref="AfterExecuteAsync(ICommandContext, IServiceProvider, CommandResult)"/>.
        /// </remarks>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="options">A range of options to customize command execution flow.</param>
        /// <returns>An empty <see cref="CommandResult"/> when <see cref="CommandExecutionOptions.ExecuteAsynchronously"/> is set to <see langword="true"/>. Otherwise, the <see cref="CommandResult"/> of the executed command.
        /// </returns>
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

        /// <summary>
        ///     Runs the command pipeline based on the input provided through the <see cref="ICommandContext"/> and options passed into this method.
        /// </summary>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>The result of the command, containing the state of failure or success.</returns>
        protected virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services)
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

        /// <summary>
        ///     Executes a resolved commandcell that succeeded through all relevant handles in the search and match pipeline.
        /// </summary>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <param name="cell">The resolved command that was chosen as the result out of the pipeline.</param>
        /// <returns>The result of the command, containing the state of failure or success.</returns>
        /// <exception cref="NotSupportedException">
        ///     Thrown when the return type of a command cannot be resolved. 
        ///     Consider overriding this method and adding the custom return type in the resolver.
        /// </exception>
        protected virtual async ValueTask<CommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, CommandCell cell)
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

        /// <summary>
        ///     A handle that is called as the final step in the command execution pipeline, publishing the outcome of the command for the developer to handle. 
        ///     This method is overridable and is the preferred way to handle command results.
        /// </summary>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <param name="result">The result of the executed command, containing failure or success and further information about it.</param>
        /// <returns></returns>
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
