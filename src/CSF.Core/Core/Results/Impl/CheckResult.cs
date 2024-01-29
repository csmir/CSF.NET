namespace CSF
{
    public readonly struct CheckResult : ICommandResult
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
