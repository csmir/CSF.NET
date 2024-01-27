using CSF.Reflection;
using CSF.Exceptions;
using CSF.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace CSF.TypeReaders
{
    public abstract class TypeReader<T> : TypeReader
    {
        public override Type Type { get; } = typeof(T);

        public override abstract ValueTask<ReadResult> EvaluateAsync(ICommandContext context, IArgument parameter, string value);
    }

    public abstract class TypeReader
    {
        private static readonly string _exHeader = "TypeReader failed to parse provided value as '{0}'. View inner exception for more details.";

        public abstract Type Type { get; }

        public abstract ValueTask<ReadResult> EvaluateAsync(ICommandContext context, IArgument parameter, string value);

        internal ValueTask<ReadResult> ObjectEvaluateAsync(ICommandContext context, IArgument parameter, object value)
        {
            if (value.GetType() == Type)
                return ValueTask.FromResult(new ReadResult(value));

            if (value is string str)
                return EvaluateAsync(context, parameter, str);

            return EvaluateAsync(context, parameter, value.ToString());
        }

        public virtual ReadResult Error([DisallowNull] Exception exception)
        {
            if (exception == null)
                ThrowHelpers.ArgMissing(exception);

            if (exception is ReadException readEx)
            {
                return new(readEx);
            }
            return new(new ReadException(string.Format(_exHeader, Type.Name), exception));
        }

        public virtual ReadResult Error([DisallowNull] string error)
        {
            if (string.IsNullOrEmpty(error))
                ThrowHelpers.ArgMissing(error);

            return new(new ReadException(error));
        }

        public virtual ReadResult Success(object value)
        {
            return new(value);
        }

        internal static TypeReader[] CreateDefaultReaders()
        {
            var range = BaseTypeReader.CreateBaseReaders();

            int length = range.Length;
            Array.Resize(ref range, length + 2);

            range[length++] = new TimeSpanTypeReader();
            range[length++] = new ColorTypeReader();

            return range;
        }
    }
}
