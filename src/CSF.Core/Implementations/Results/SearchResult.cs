using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSF
{
    public readonly struct SearchResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The commands that matched the search best, and will be used for continuing the pipeline.
        /// </summary>
        public IEnumerable<Command> Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private SearchResult(bool success, IEnumerable<Command> matches = null, string msg = null, Exception exception = null)
        {
            Result = matches;
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<SearchResult>(SearchResult result)
            => result.AsValueTask();

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
        public static SearchResult FromSuccess(IEnumerable<Command> matches)
            => new SearchResult(true, matches);
    }
}
