namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        ///     Evaluates a condition to handle a command and returns the result.
        /// </summary>
        /// <param name="context">The command context used to execute the command currently in scope.</param>
        /// <param name="command">The command that is to be executed if this and all other precondition evaluations succeed.</param>
        /// <param name="services">The services in scope for the current command execution.</param>
        /// <returns>A result that represents the outcome of the evaluation.</returns>
        public abstract async Task<bool> EvaluateAsync(ICommandContext context, Command command);

        /// <summary>
        ///     Returns that the evaluation has failed.
        /// </summary>
        /// <param name="reason">The reason why the evaluation failed.</param>
        protected static Result Failure(string reason)
            => new(reason);

        /// <summary>
        ///     Returns that the evaluation has succeeded.
        /// </summary>
        protected static Result Success()
            => new();

        /// <summary>
        ///     Represents the result structure that displays the returned state of the evaluation.
        /// </summary>
        public readonly struct Result
        {
            /// <summary>
            ///     Gets if the evaluation was successful or not.
            /// </summary>
            public bool IsSuccess { get; }

            /// <summary>
            ///     Gets the reason of failure in case it has.
            /// </summary>
            public string Reason { get; }

            /// <summary>
            ///     Creates a new successful evaluation result.
            /// </summary>
            public Result()
            {
                IsSuccess = true;
                Reason = null;
            }

            /// <summary>
            ///     Creates a new failed evaluation result.
            /// </summary>
            /// <param name="reason">The reason why the evaluation has failed.</param>
            public Result(string reason)
            {
                IsSuccess = false;
                Reason = reason;
            }
        }
    }
}
