using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a result returned by checking a range of commands.
    /// </summary>
    public readonly struct MatchResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The commands that passed through the preconditions, the first of which will be selected for execution.
        /// </summary>
        public IEnumerable<Command> Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private MatchResult(bool success, IEnumerable<Command> matches, string msg = null, Exception exception = null)
        {
            Result = matches;
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<MatchResult>(MatchResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static MatchResult Error(string errorMessage, Exception exception = null)
            => new(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static MatchResult Success(IEnumerable<Command> matches)
            => new(true, matches);
    }
}
