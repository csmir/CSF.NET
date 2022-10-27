using System;

namespace CSF
{
    /// <summary>
    ///     Represents search results.
    /// </summary>
    public readonly struct SearchResult : IResult
    {
        public bool IsSuccess { get; }

        public string ErrorMessage { get; }

        /// <summary>
        ///     The command that matched the search best, and will be used for continuing the pipeline.
        /// </summary>
        internal Command Match { get; }

        public Exception Exception { get; }

        private SearchResult(bool success, Command match = null, string msg = null, Exception exception = null)
        {
            Match = match;
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static SearchResult FromError(string errorMessage, Exception exception = null)
            => new SearchResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        internal static SearchResult FromSuccess(Command match)
            => new SearchResult(true, match);
    }
}
