namespace CSF
{
    /// <summary>
    ///     Represents a class that's used to describe data from the command.
    /// </summary>
    public class CommandContext<T>(T options) : ICommandContext
        where T : IExecutionOptions
    {
        public IExecutionOptions Options { get; } = options;
    }
}
