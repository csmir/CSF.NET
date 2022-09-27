using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Represents module building results.
    /// </summary>
    public readonly struct BuildResult : IResult
    {
        public bool IsSuccess { get; }

        public string ErrorMessage { get; }

        public Exception Exception { get; }

        private BuildResult(bool success, string errorMessage = null, Exception exception = null)
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
        public static BuildResult FromError(string errorMessage, Exception exception = null)
            => new BuildResult(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static BuildResult FromSuccess()
            => new BuildResult(true);
    }
}
