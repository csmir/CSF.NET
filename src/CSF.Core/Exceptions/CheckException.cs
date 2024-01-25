namespace CSF.Exceptions
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when no matched command succeeded its precondition checks.
    /// </summary>
    public sealed class CheckException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {
    }
}
