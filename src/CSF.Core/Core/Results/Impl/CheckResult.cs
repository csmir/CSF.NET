﻿namespace CSF
{
    /// <summary>
    ///     Represents the result of a check operation within the command execution pipeline.
    /// </summary>
    public readonly struct CheckResult : ICommandResult
    {
        /// <inheritdoc />
        public Exception Exception { get; } = null;

        /// <inheritdoc />
        public bool Success { get; } = true;

        internal CheckResult(Exception exception)
        {
            Exception = exception;
            Success = false;
        }
    }
}
