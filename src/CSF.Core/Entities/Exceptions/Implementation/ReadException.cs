namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="PipelineException"/> that is thrown when no matched command succeeded parsing its parameters.
    /// </summary>
    public sealed class ReadException : PipelineException
    {
        public ReadException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override FailedResult AsResult()
        {
            return new(FailureCode.Read, this);
        }
    }
}
