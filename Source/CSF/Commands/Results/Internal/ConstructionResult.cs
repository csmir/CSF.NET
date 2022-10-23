using System;

namespace CSF
{
    /// <summary>
    ///     Represents constructor results.
    /// </summary>
    public readonly struct ConstructionResult : IResult
    {
        public bool IsSuccess { get; }

        public string ErrorMessage { get; }

        /// <summary>
        ///     The result object of this reader.
        /// </summary>
        internal ICommandBase Result { get; }

        public Exception Exception { get; }

        private ConstructionResult(bool success, ICommandBase result = null, string msg = null, Exception exception = null)
        {
            IsSuccess = success;
            ErrorMessage = msg;
            Exception = exception;
            Result = result;
        }

        /// <summary>
        ///     Creates a failed result with provided parameters.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ConstructionResult FromError(string errorMessage, Exception exception = null)
            => new ConstructionResult(false, null, errorMessage, exception);

        /// <summary>
        ///     Creates a succesful result with provided parameters.
        /// </summary>
        /// <returns></returns>
        internal static ConstructionResult FromSuccess(ICommandBase value)
            => new ConstructionResult(true, value);
    }
}
