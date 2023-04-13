namespace CSF
{
    public sealed class ReadException : PipelineException
    {
        public ReadException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override IResult AsResult()
        {
            return new Result(FailureCode.Read, Message, this);
        }
    }
}
