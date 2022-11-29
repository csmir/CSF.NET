using System;

namespace CSF
{
    /// <summary>
    ///     Represents a logger to write logs to.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     The log level of this logger. Can be changed during runtime to modify the log severity away from the default severity defined in <see cref="CommandConfiguration.DefaultLogLevel"/>
        /// </summary>
        LogLevel LogLevel { get; set; }

        /// <summary>
        ///     The resolver that pushes log messages to the target 
        /// </summary>
        LogResolver Resolver { get; set; }

        /// <summary>
        ///     Writes a trace log.
        /// </summary>
        /// <remarks>
        ///     This will not be written if the <see cref="LogLevel"/> is above <see cref="LogLevel.Trace"/>.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void WriteTrace(object message, Exception exception = null);

        /// <summary>
        ///     Writes a debug log.
        /// </summary>
        /// <remarks>
        ///     This will not be written if the <see cref="LogLevel"/> is above <see cref="LogLevel.Debug"/>.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void WriteDebug(object message, Exception exception = null);

        /// <summary>
        ///     Writes an information log.
        /// </summary>
        /// <remarks>
        ///     This will not be written if the <see cref="LogLevel"/> is above <see cref="LogLevel.Information"/>.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void WriteInfo(object message, Exception exception = null);

        /// <summary>
        ///     Writes a warning log.
        /// </summary>
        /// <remarks>
        ///     This will not be written if the <see cref="LogLevel"/> is above <see cref="LogLevel.Warning"/>.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void WriteWarning(object message, Exception exception = null);

        /// <summary>
        ///     Writes an error log.
        /// </summary>
        /// <remarks>
        ///     This will not be written if the <see cref="LogLevel"/> is above <see cref="LogLevel.Error"/>.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void WriteError(object message, Exception exception = null);

        /// <summary>
        ///     Writes a critical log.
        /// </summary>
        /// <remarks>
        ///     This will not be written if the <see cref="LogLevel"/> is above <see cref="LogLevel.Critical"/>.
        /// </remarks>
        /// <param name="message"></param>
        /// <param name="values"></param>
        void WriteCritical(object message, Exception exception = null);

        /// <summary>
        ///     Writes a log.
        /// </summary>
        /// <remarks>
        ///     Will not be written if the <see cref="LogLevel"/> is above the defined level in the <paramref name="log"/>.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="log"></param>
        void Write<T>(T log)
            where T : ILog;
    }
}
