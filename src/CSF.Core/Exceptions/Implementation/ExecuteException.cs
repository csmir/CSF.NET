namespace CSF
{
    public sealed class ExecuteException : PipelineException
    {
        public ExecuteException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override IResult AsResult()
        {
            return new Result(FailureCode.Execute, Message, this);
        }
    }
}
