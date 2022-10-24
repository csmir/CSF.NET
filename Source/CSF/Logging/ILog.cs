using System;

namespace CSF
{
    /// <summary>
    ///     Represents a log message fired from <see cref="CommandFramework.Log"/>.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        ///     The message of this log.
        /// </summary>
        object Value { get; }

        /// <summary>
        ///     The severity of this log.
        /// </summary>
        LogLevel LogLevel { get; }

        /// <summary>
        ///     The exception of this log. <see langword="null"/> if none.
        /// </summary>
        Exception Exception { get; }
    }
}
