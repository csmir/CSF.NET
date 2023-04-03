using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSF
{
    public readonly struct CheckYieldResult : IResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <inheritdoc/>
        public string ErrorMessage { get; }

        /// <summary>
        ///     The commands that passed through the preconditions, the first of which will be selected for execution.
        /// </summary>
        public IEnumerable<(Command, ArgsResult)> Result { get; }

        /// <inheritdoc/>
        public Exception Exception { get; }

        private CheckYieldResult(bool success, IEnumerable<(Command, ArgsResult)> matches, string msg = null, Exception exception = null)
        {
            Result = matches;
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
        }

        public static implicit operator ValueTask<CheckYieldResult>(CheckYieldResult result)
            => result.AsValueTask();

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static CheckYieldResult FromError(string errorMessage, Exception exception = null)
            => new CheckYieldResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static CheckYieldResult FromSuccess(IEnumerable<(Command, ArgsResult)> matches)
            => new CheckYieldResult(true, matches);
    }
}
