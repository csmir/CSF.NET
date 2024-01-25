namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when no command could be found.
    /// </summary>
    public sealed class SearchException(string message, Exception innerException = null) 
        : ExecutionException(message, innerException)
    {
    }
}
