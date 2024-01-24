namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when no command could be found.
    /// </summary>
    public sealed class SearchException : ExecutionException
    {
        public SearchException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
