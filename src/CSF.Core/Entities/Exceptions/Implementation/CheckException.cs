namespace CSF
{
    public sealed class CheckException : PipelineException
    {
        public CheckException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override CommandResult AsResult()
        {
            return new CommandResult(ResultCode.Check, Message, this);
        }
    }
}
