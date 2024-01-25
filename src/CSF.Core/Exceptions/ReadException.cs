namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when no matched command succeeded parsing its parameters.
    /// </summary>
    public sealed class ReadException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {
    }
}
