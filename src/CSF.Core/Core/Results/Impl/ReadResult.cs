namespace CSF
{
    public readonly struct ReadResult : IResult
    {
        public Exception Exception { get; } = null;

        public object Value { get; }

        public bool Success { get; }

        internal ReadResult(object value)
        {
            Value = value;
            Success = true;
        }

        internal ReadResult(Exception exception)
        {
            Exception = exception;
            Value = null;
            Success = false;
        }
    }
}
