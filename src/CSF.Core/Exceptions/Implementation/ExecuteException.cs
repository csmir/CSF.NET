namespace CSF
{
    public sealed class ExecuteException : PipelineException
    {
        public ExecuteException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override CommandResult AsResult()
        {
            return new CommandResult(FailedStep.Execute, Message, this);
        }
    }
}
