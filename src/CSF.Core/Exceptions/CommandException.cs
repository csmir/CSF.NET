namespace CSF.Exceptions
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when the command being executed failed to run its body.
    /// </summary>
    public sealed class CommandException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {
    }
}
