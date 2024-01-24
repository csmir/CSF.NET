using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CSF
{
    internal static class ThrowHelpers
    {
        [DoesNotReturn]
        public static void InvalidOp([DisallowNull] string failureMessage)
        {
            throw new InvalidOperationException(failureMessage);
        }

        [DoesNotReturn]
        public static void InvalidArg(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            throw new ArgumentException("Argument is not in valid state, being null, empty or whitespace.", paramName: arg);
        }
    }
}
