namespace CSF
{
    public sealed class CheckException : PipelineException
    {
        public CheckException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override IResult AsResult()
        {
            return new Result(FailureCode.Check, Message, this);
        }
    }
}
