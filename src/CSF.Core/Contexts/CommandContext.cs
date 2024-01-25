namespace CSF
{
    public class CommandContext<T>(T options) : CommandContext(options), ICommandContext
        where T : IExecutionOptions
    {
        public new T Options { get; } = options;

        IExecutionOptions ICommandContext.Options { get; } = options;
    }

    public class CommandContext(IExecutionOptions options) : ICommandContext
    {
        public IExecutionOptions Options { get; } = options;
    }
}
