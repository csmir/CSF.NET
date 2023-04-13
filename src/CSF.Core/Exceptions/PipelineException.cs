namespace CSF
{
    public abstract class PipelineException : Exception
    {
        public PipelineException(string message)
            : base(message)
        {

        }

        public PipelineException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }

        public abstract IResult AsResult();
    }
}
