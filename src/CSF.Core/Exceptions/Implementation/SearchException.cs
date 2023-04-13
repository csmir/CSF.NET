namespace CSF
{
    public sealed class SearchException : PipelineException
    {
        public SearchException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override IResult AsResult()
        {
            return new Result(FailureCode.Search, Message, this);
        }
    }
}
