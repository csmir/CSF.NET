namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Executes a command based on the input provided through the <see cref="ICommandContext"/> and options passed into this method.
        /// </summary>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="options">A range of options to customize command execution flow.</param>
        /// <returns>An <see cref="IResult"/> that can be awaited in asynchronous context.
        /// </returns>
        public virtual IResult ExecuteAsync(ICommandContext context, CommandExecutionOptions options = null)
        {
            options ??= CommandExecutionOptions.Default;

            var services = options.Scope?.ServiceProvider ?? _services;

            return ExecuteAsync(context, services);
        }

        /// <summary>
        ///     Runs the command pipeline based on the input provided through the <see cref="ICommandContext"/> and options passed into this method.
        /// </summary>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>The result of the command, containing the state of failure or success.</returns>
        protected virtual IResult ExecuteAsync(ICommandContext context, IServiceProvider services)
        {
            try
            {
                var cell = Search(context, services);

                return ExecuteAsync(context, services, cell);
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
        protected virtual IResult ExecuteAsync(ICommandContext context, IServiceProvider services, CommandCell cell)
        {
            var value = cell.Execute(context, services);

            switch (value)
            {
                case Task task:
                    return new SuccessResult(task);
                case null:
                    return new SuccessResult();
                default:
                    throw new NotSupportedException("The return value of this command is not supported.");
            }
        }
    }

    internal static class ExecuteOperations
    {
        public static object Execute(this CommandCell cell, ICommandContext context, IServiceProvider services)
        {
            var module = services.GetService(cell.Command.Module.Type) as ModuleBase;

            module.Context = context;
            module.Command = cell.Command;
            module.Services = services;

            var value = cell.Command.Target.Invoke(module, cell.Arguments);

            return value;
        }
    }
}
