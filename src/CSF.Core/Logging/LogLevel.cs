namespace CSF
{
    public enum LogLevel : byte
    {
        /// <summary>
        ///     Defines that the <see cref="ILog"/> should not be logged.
        /// </summary>
        None = 6,

        /// <summary>
        ///     Defines a critical process log. When a critical log is pushed through the log processor, it will // TODO
        /// </summary>
        Critical = 5,

        /// <summary>
        ///     Defines an error in the app process. This is often due to developer-side errors.
        /// </summary>
        Error = 4,

        /// <summary>
        ///     Defines a warning in the app process, occuring when certain specified values are not properly resolved.
        /// </summary>
        Warning = 3,

        /// <summary>
        ///     Defines an informational log about the duration of executions and the steps it takes.
        /// </summary>
        Information = 2,

        /// <summary>
        ///     Defines a debugging log. This log exposes information about the pipeline and information process of the application at its current state.
        /// </summary>
        /// <remarks>
        ///     This level of logging should not be used in production for it only exposes data for investigative purposes.
        /// </remarks>
        Debug = 1,

        /// <summary>
        ///     Defines a trace log. This log level should only be used in local testing and never in production, as it may show sensitive data.
        /// </summary>
        /// <remarks>
        ///     As a safety check, this log level is not accessible in the release build state, and will default to debug instead.
        /// </remarks>
        Trace = 0,
    }
}
