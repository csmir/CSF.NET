namespace CSF
{
    /// <summary>
    ///     Represents an interface that supports all implementations of command context classes.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        ///     The command options.
        /// </summary>
        public IExecutionOptions Options { get; }
    }
}
