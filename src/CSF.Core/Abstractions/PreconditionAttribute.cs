using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class PreconditionAttribute : Attribute
    {
        private static readonly string _exHeader = "Precondition result halted further command execution. View inner exception for more details.";
        /// <summary>
        ///     Evaluates a condition to handle a command and returns the result.
        /// </summary>
        /// <param name="context">The command context used to execute the command currently in scope.</param>
        /// <param name="command">The command that is to be executed if this and all other precondition evaluations succeed.</param>
        /// <param name="services">The services in scope for the current command execution.</param>
        /// <returns>A result that represents the outcome of the evaluation.</returns>
        public abstract ValueTask<CheckResult> EvaluateAsync(ICommandContext context, Command command);

        public static CheckResult Error([DisallowNull] Exception exception)
        {
            if (exception == null)
                ThrowHelpers.ArgMissing(exception);

            if (exception is CheckException checkEx)
            {
                return new(checkEx);
            }
            return new(new CheckException(_exHeader, exception));
        }

        public virtual CheckResult Error([DisallowNull] string error)
        {
            if (string.IsNullOrEmpty(error))
                ThrowHelpers.ArgMissing(error);

            return new(new CheckException(error));
        }

        public virtual CheckResult Success()
        {
            return new();
        }
    }
}
