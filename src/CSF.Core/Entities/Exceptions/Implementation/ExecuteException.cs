namespace CSF
{
    /// <summary>
    ///     Represents a <see cref="PipelineException"/> that is thrown when the command being executed failed to run its body.
    /// </summary>
    public sealed class ExecuteException : PipelineException
    {
        public ExecuteException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override CommandResult AsResult()
        {
            return new(ResultCode.Execute, this);
        }
    }
}
