using System;

namespace CSF
{
    /// <summary>
    ///     Represents internal failures in commands.
    /// </summary>
    public readonly struct CommandResult : IResult
    {
        public bool IsSuccess { get; }

        public string Message { get; }

        public Exception Exception { get; }

        private CommandResult(bool success, string msg = null, Exception exception = null)
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
        public static IResult FromError(string errorMessage, Exception exception = null)
            => new CommandResult(false, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        public static IResult FromSuccess()
            => new CommandResult(true);
    }
}
