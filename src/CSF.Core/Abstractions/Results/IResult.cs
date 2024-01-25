namespace CSF
{
    public interface IResult
    {
        public Exception Exception { get; }

        public bool Success { get; }
    }
}
