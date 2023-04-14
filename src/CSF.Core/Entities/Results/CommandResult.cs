namespace CSF
{
    public readonly struct CommandResult
    {
        /// <inheritdoc/>
        public FailedStep Step { get; } = FailedStep.None;

        /// <inheritdoc/>
        public string Message { get; } = null;

        /// <inheritdoc/>
        public Exception Exception { get; } = null;

        public CommandResult(FailedStep step, string message = null, Exception exception = null)
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
