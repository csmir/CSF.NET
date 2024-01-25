using CSF.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CSF.Helpers
{
    internal static class ThrowHelpers
    {
        [DoesNotReturn]
        public static void InvalidOp([DisallowNull] string failureMessage)
        {
            throw new InvalidOperationException(failureMessage);
        }

        [DoesNotReturn]
        public static void ArgMissing(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            throw new ArgumentMissingException(arg, "Argument is not in valid state, being null, empty or whitespace.");
        }

        [DoesNotReturn]
        public static void RangeDuplicate(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            throw new RangeDuplicateException(arg, "Range contains a duplicate value, which is not supported by the implementation.");
        }
    }
}
