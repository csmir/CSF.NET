using System;

namespace CSF
{
    /// <summary>
    ///     Represents internal failures when creating and running modules.
    /// </summary>
    public readonly struct ModuleResult : IResult
    {
        public bool IsSuccess { get; }

        public string Message { get; }

        public Exception Exception { get; }

        private ModuleResult(bool success, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            Message = msg;
            Exception = exception;
        }

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static IResult FromError(string errorMessage, Exception exception = null)
            => new ModuleResult(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static IResult FromSuccess()
            => new ModuleResult(true);
    }
}
