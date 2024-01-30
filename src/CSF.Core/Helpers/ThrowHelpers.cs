using CSF.Exceptions;
using System.Collections;
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
        public static void InvalidArg(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            // should resolve overload when a collection is null or empty.
            if (value is ICollection or IEnumerable)
            {
                // if invalidarg is called on not null collection, it must mean it needs a value.
                if (value != null)
                {
                    throw new ArgumentException(
                        message: "Provided collection must be initialized with at least one argument. It is not allowed to be null or empty.",
                        paramName: arg);
                }

                // thrown when collection is null.
                throw new ArgumentException(
                    message: "Provided collection must be initialized. It is not allowed to be null.", 
                    paramName: arg);
            }

            // if value is a string, this check ensures that more data is provided at throw, regarding the state of the string.
            if (value is string)
            {
                throw new ArgumentMissingException(
                    message: "Provided string must carry at least a character that is not whitespace. It is not allowed to be null or empty.", 
                    paramName: arg);
            }

            // throw argument null with custom message in the situation where none of the above overloads are relevant.
            throw new ArgumentException(
                message: "Provided argument must carry a value. It is not allowed to be null.",
                paramName: arg);
        }

        [DoesNotReturn]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void NotDistinct(object value, [CallerArgumentExpression(nameof(value))] string arg = null)
        {
            throw new ArgumentException(
                message: "Provided collection cannot contain duplicate values.", 
                paramName: arg);
        }
    }
}
