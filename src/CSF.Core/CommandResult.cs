namespace CSF
{
    public enum ResultCode : int
    {
        None,

        Search,

        Read,

        Check,

        Parse,

        Execute,
    }

    public readonly struct CommandResult
    {
        /// <inheritdoc/>
        public ResultCode Step { get; } = ResultCode.None;

        /// <inheritdoc/>
        public string Message { get; } = null;

        /// <inheritdoc/>
        public Exception Exception { get; } = null;

        public CommandResult(ResultCode step, string message = null, Exception exception = null)
        {
            Step = step;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        ///     Formats a readable output from the result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"[{Step}]: {Message}" + Exception != null ? $"\n\r{Exception}" : "";
    }
}
