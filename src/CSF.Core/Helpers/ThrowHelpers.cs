using CSF.Exceptions;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CSF.Helpers
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class ThrowHelpers
    {
        [DoesNotReturn]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InvalidOp([DisallowNull] string failureMessage)
        {
            throw new InvalidOperationException(failureMessage);
        }

        [DoesNotReturn]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ArgMissing(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            throw new ArgumentMissingException(arg, "Argument is not in valid state, being null, empty or whitespace.");
        }

        [DoesNotReturn]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RangeDuplicate(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            throw new RangeDuplicateException(arg, "Range contains a duplicate value, which is not supported by the implementation.");
        }
    }
}
