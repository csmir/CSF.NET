namespace CSF
{
    /// <summary>
    ///     An interface that represents a command flow precondition.
    /// </summary>
    public interface IPrecondition
    {
        public abstract void Check(IContext context, Command command, IServiceProvider provider);
    }
}
