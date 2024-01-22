namespace CSF
{
    public class MatchException : ExecutionException
    {
        public MatchException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public override FailedResult AsResult()
        {
            return new(FailureCode.Execute, this);
        }
    }
}
