namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Checks all existing preconditions on the specified <see cref="Command"/> passed into the method.
        /// </summary>
        /// <param name="command">The command to check all preconditions for.</param>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>An empty <see cref="FailedResult"/> if the precondition evaluations succeeded. If any failed, the result of the first failed precondition.</returns>
        protected virtual FailedResult Check(Command command, ICommandContext context, IServiceProvider services)
        {
            try
            {
                command.Check(context, services);

                return new();
            }
            catch (PipelineException e)
            {
                return e.AsResult();
            }
        }
    }

    internal static class CheckOperations
    {
        public static void Check(this Command command, ICommandContext context, IServiceProvider services)
        {
            if (command.HasPreconditions)
                Parallel.ForEach(command.Preconditions, x => x.EvalInternal(context, command, services));
        }
    }
}
