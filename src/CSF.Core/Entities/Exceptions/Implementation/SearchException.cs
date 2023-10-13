namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="PipelineException"/> that is thrown when no command could be found.
    /// </summary>
    public sealed class SearchException : PipelineException
    {
        public SearchException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override FailedResult AsResult()
        {
            return new(FailureCode.Search, this);
        }
    }
}
