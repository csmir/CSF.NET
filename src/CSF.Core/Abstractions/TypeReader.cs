using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     Represents a generic <see cref="TypeReader{T}"/> to use for parsing provided types into the targetted type.
    /// </summary>
    /// <typeparam name="T">The targetted type for this typereader.</typeparam>
    public abstract class TypeReader<T> : TypeReader
    {
        /// <inheritdoc />
        public override Type Type { get; } = typeof(T);

        /// <inheritdoc />
        public override abstract ValueTask<ReadResult> EvaluateAsync(ICommandContext context, IParameterComponent parameter, string value);
    }

    public abstract class TypeReader
    {
        private static readonly string _exHeader = "TypeReader failed to parse provided value as '{0}'. View inner exception for more details.";

        /// <summary>
        ///     The type that this reader intends to return.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        ///     Evaluates an input and tries to parse it into a value that matches the expected parameter type.
        /// </summary>
        /// <param name="context">The command context used to execute the command currently in scope.</param>
        /// <param name="parameter">The parameter this input evaluation is targetting.</param>
        /// <param name="services">The services in scope for the current command execution.</param>
        /// <param name="value">The input that this evaluation intends to convert into the expected parameter type.</param>
        /// <returns>A result that represents the outcome of the evaluation.</returns>
        public abstract ValueTask<ReadResult> EvaluateAsync(ICommandContext context, IParameterComponent parameter, string value);

        internal ValueTask<ReadResult> ObjectEvaluateAsync(ICommandContext context, IParameterComponent parameter, object value)
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
