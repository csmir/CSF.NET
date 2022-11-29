using System;

namespace CSF
{
    /// <summary>
    ///     The default logger. This logger does not support overrides. Implement <see cref="ILogger"/> for message format customization.
    /// </summary>
    public sealed class DefaultLogger : ILogger
    {
        /// <summary>
        ///     The log level inherited from the <see cref="CommandConfiguration"/>. This value can be overwritten at runtime to change the default severity for all log messages.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        ///     The default resolver inherited from 
        /// </summary>
        public LogResolver Resolver { get; set; }

        public DefaultLogger(LogLevel level, LogResolver resolver = null)
        {
            LogLevel = level;

            if (resolver is null)
                resolver = LogResolver.CreateLocalProvider();

            Resolver = resolver;
        }

        public void WriteTrace(object message, Exception exception = null)
            => Write(new Log(LogLevel.Trace, message, exception));

        public void WriteDebug(object message, Exception exception = null)
            => Write(new Log(LogLevel.Debug, message, exception));

        public void WriteInfo(object message, Exception exception = null)
            => Write(new Log(LogLevel.Information, message, exception));

        public void WriteWarning(object message, Exception exception = null)
            => Write(new Log(LogLevel.Warning, message, exception));

        public void WriteError(object message, Exception exception)
            => Write(new Log(LogLevel.Error, message, exception));

        public void WriteCritical(object message, Exception exception)
            => Write(new Log(LogLevel.Critical, message, exception));

        public void Write<T>(T log)
            where T : ILog
        {
            if (LogLevel is LogLevel.None)
                return;

            if (log.LogLevel >= LogLevel)
            {
                Resolver.Send(log);
            }

            if (log.LogLevel is LogLevel.Critical)
                ResolveCriticalException(log);
        }

        private void ResolveCriticalException<T>(T log)
            where T : ILog
        {
            throw new InvalidOperationException("A critical exception has occurred.", log.Exception);
        }
    }
}
