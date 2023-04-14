namespace CSF
{
    public sealed class ReadException : PipelineException
    {
        public ReadException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override CommandResult AsResult()
        {
            return new CommandResult(ResultCode.Read, Message, this);
        }
    }
}
