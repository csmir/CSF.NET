namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="PipelineException"/> that is thrown when no matched command succeeded its precondition checks.
    /// </summary>
    public sealed class CheckException : PipelineException
    {
        public CheckException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override FailedResult AsResult()
        {
            return new(FailureCode.Check, this);
        }
    }
}
