using System;
using System.Collections.Generic;
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
        ///     The result objects of the read handle.
        /// </summary>
        public IEnumerable<object> Result { get; }

        /// <summary>
        ///     The current index placement for the read handle.
        /// </summary>
        internal int Placement { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private ArgsResult(bool success, IEnumerable<object> result = null, int index = -1, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
            Result = result;
            Placement = index;
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
            => new ArgsResult(false, null, -1, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static ArgsResult FromSuccess(IEnumerable<object> value, int index)
            => new ArgsResult(true, value, index);
    }
}
