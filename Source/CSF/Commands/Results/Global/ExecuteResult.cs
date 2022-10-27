using System;

namespace CSF
{
    /// <summary>
    ///     Represents internal failures in commands.
    /// </summary>
    public readonly struct ExecuteResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private ExecuteResult(bool success, string errorMessage = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ExecuteResult FromError(string errorMessage, Exception exception = null)
            => new ExecuteResult(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static ExecuteResult FromSuccess()
            => new ExecuteResult(true);
    }
}
