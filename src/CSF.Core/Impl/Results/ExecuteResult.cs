using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a result returned by command execution.
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

        public static implicit operator ValueTask<ExecuteResult>(ExecuteResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ExecuteResult Error(string errorMessage, Exception exception = null)
            => new(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static ExecuteResult Success()
            => new(true);
    }
}
