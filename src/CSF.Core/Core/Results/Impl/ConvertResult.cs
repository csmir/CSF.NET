﻿namespace CSF
{
    /// <summary>
    ///     The result of a convert operation within the command execution pipeline.
    /// </summary>
    public readonly struct ConvertResult : ICommandResult
    {
        /// <inheritdoc />
        public Exception Exception { get; } = null;

        /// <inheritdoc />
        public bool Success { get; }

        internal object Value { get; } = null;

        internal ConvertResult(object value)
        {
            Value = value;
            Success = true;
        }

        internal ConvertResult(Exception exception)
        {
            Exception = exception;
            Success = false;
        }
    }
}
