namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="ExecutionException"/> that is thrown when no matched command succeeded its precondition checks.
    /// </summary>
    public sealed class CheckException : ExecutionException
    {
        public CheckException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
