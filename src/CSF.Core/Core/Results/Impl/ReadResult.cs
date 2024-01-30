namespace CSF
{
    /// <summary>
    ///     Represents the result of a read operation within the command execution pipeline.
    /// </summary>
    public readonly struct ReadResult : ICommandResult
    {
        /// <inheritdoc />
        public Exception Exception { get; } = null;

        /// <inheritdoc />
        public bool Success { get; }

        internal object Value { get; } = null;

        internal ReadResult(object value)
        {
            Value = value;
            Success = true;
        }

        internal ReadResult(Exception exception)
        {
            Exception = exception;
            Success = false;
        }
    }
}
