namespace CSF
{
    public sealed class SearchException : PipelineException
    {
        public SearchException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override CommandResult AsResult()
        {
            return new CommandResult(ResultCode.Search, Message, this);
        }
    }
}
