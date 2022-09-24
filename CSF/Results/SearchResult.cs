using System;

namespace CSF
{
    /// <summary>
    ///     Represents search results.
    /// </summary>
    public readonly struct SearchResult : IResult
    {
        public bool IsSuccess { get; }

        public string Message { get; }

        public Exception Exception { get; }

        private SearchResult(bool success, string msg = null, Exception exception = null)
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
        public static SearchResult FromError(string errorMessage, Exception exception = null)
            => new SearchResult(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static SearchResult FromSuccess()
            => new SearchResult(true);
    }
}
