using System;

namespace CSF
{
    /// <summary>
    ///     Represents a logger to write logs to.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     The log level inherited from the <see cref="CommandConfiguration"/>. This value can be overwritten at runtime to change the default severity for all log messages.
        /// </summary>
        LogLevel LogLevel { get; set; }

        /// <summary>
        ///     The action used to log the provided response.
        /// </summary>
        Action<Log> SendAction { get; set; }

        /// <summary>
        ///     Writes a trace log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void Trace(object message, Exception exception = null);

        /// <summary>
        ///     Writes a debug log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void Debug(object message, Exception exception = null);

        /// <summary>
        ///     Writes an information log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void Info(object message, Exception exception = null);

        /// <summary>
        ///     Writes a warning log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void Warning(object message, Exception exception = null);

        /// <summary>
        ///     Writes an error log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void Error(object message, Exception exception = null);

        /// <summary>
        ///     Writes a critical log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void Critical(object message, Exception exception = null);

        /// <summary>
        ///     Writes a log.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        void Write(Log log);

        /// <summary>
        ///     Uses the <see cref="SendAction"/> to send the provided message to the target handler.
        /// </summary>
        /// <param name="message"></param>
        void Send(Log message);
    }
}
