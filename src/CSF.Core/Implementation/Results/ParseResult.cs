using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a result returned by parsing a command.
    /// </summary>
    public readonly struct ParseResult : IResult
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

        private ParseResult(bool success, IEnumerable<object> result = null, int index = -1, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
            Result = result;
            Placement = index;
        }

        public static implicit operator ValueTask<ParseResult>(ParseResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ParseResult Error(string errorMessage, Exception exception = null)
            => new ParseResult(false, null, -1, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static ParseResult Success(IEnumerable<object> value, int index)
            => new ParseResult(true, value, index);
    }
}
