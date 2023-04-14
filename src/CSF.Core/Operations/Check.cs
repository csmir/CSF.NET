namespace CSF
{
    public partial class CommandManager
    {
        public virtual CommandResult Check(Command command, IContext context, IServiceProvider services)
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
        public static void Check(this Command command, IContext context, IServiceProvider services)
        {
            foreach (var precondition in command.Preconditions)
                precondition.Check(context, command, services);
        }
    }
}
