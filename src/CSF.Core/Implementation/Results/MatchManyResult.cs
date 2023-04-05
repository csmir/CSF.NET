using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a result returned by grading a range of commands.
    /// </summary>
    public readonly struct MatchManyResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The commands that passed through the preconditions, the first of which will be selected for execution.
        /// </summary>
        public IEnumerable<(Command, ParseResult)> Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private MatchManyResult(bool success, IEnumerable<(Command, ParseResult)> matches, string msg = null, Exception exception = null)
        {
            Result = matches;
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<MatchManyResult>(MatchManyResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static MatchManyResult Error(string errorMessage, Exception exception = null)
            => new MatchManyResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static MatchManyResult Success(IEnumerable<(Command, ParseResult)> matches)
            => new MatchManyResult(true, matches);
    }
}
