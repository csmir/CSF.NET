namespace CSF
{
    public readonly struct CheckResult : IResult
    {
        public Exception Exception { get; } = null;

        public bool Success { get; } = true;

        internal CheckResult(Exception exception)
        {
            Exception = exception;
            Success = false;
        }
    }
}
