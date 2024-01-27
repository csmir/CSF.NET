using CSF.Reflection;
using CSF.Exceptions;
using CSF.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Preconditions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class PreconditionAttribute : Attribute
    {
        private static readonly string _exHeader = "Precondition result halted further command execution. View inner exception for more details.";

        public abstract ValueTask<CheckResult> EvaluateAsync(ICommandContext context, CommandInfo command);

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
