using System;

namespace CSF.Results
{
    /// <summary>
    ///     Represents type reader results.
    /// </summary>
    public readonly struct TypeReaderResult : IResult
    {
        public bool IsSuccess { get; }

        public string Message { get; }

        /// <summary>
        ///     The result object of this reader.
        /// </summary>
        public object Result { get; }

        public Exception Exception { get; }

        private TypeReaderResult(bool success, object result = null, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            Message = msg;
            Exception = exception;
            Result = result;
        }

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static TypeReaderResult FromError(string errorMessage, Exception exception = null)
            => new TypeReaderResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static TypeReaderResult FromSuccess(object value)
            => new TypeReaderResult(true, value);
    }
}
