using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents typereader parsing results.
    /// </summary>
    public readonly struct ArgsResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The result objects of this parse result.
        /// </summary>
        internal object[] Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private ArgsResult(bool success, object[] result = null, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
            Result = result;
        }

        public static implicit operator ValueTask<ArgsResult>(ArgsResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ArgsResult FromError(string errorMessage, Exception exception = null)
            => new ArgsResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static ArgsResult FromSuccess(object[] value)
            => new ArgsResult(true, value);
    }
}
