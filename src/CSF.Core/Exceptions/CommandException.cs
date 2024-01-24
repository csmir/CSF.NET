namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when the command being executed failed to run its body.
    /// </summary>
    public sealed class CommandException : ExecutionException
    {
        public CommandException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
