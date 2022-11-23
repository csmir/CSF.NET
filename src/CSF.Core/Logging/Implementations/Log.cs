using System;

namespace CSF
{
    /// <summary>
    ///     Represents a default log message for the core framework.
    /// </summary>
    public readonly struct Log : ILog
    {
        /// <inheritdoc/>
        public object Value { get; }

        /// <inheritdoc/>
        public LogLevel LogLevel { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        /// <summary>
        ///     Creates a new log with the provided severity.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="value"></param>
        /// <param name="exception"></param>
        public Log(LogLevel level, object value, Exception exception = null)
        {
            Value = value;
            LogLevel = level;
            Exception = exception;
        }
    }
}
