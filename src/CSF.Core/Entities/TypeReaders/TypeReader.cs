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
        public override abstract Result Evaluate(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value);
    }

    public abstract class TypeReader
    {
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
        public abstract Result Evaluate(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value);

        /// <summary>
        ///     Returns that the evaluation has failed.
        /// </summary>
        /// <param name="reason">The reason why the evaluation failed.</param>
        protected static Result Failure(string reason)
            => new(reason);

        /// <summary>
        ///     Returns that the evaluation has succeeded.
        /// </summary>
        /// <param name="value">The value the evaluation returns upon success.</param>
        protected static Result Success(object value)
            => new(value);

        internal object EvalInternal(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            var result = Evaluate(context, parameter, services, value);

            if (!result.IsSuccess)
                throw new ReadException(result.Reason);

            return result.Value;
        }

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
            ///     Gets the value the evaluation returned if it succeeded.
            /// </summary>
            public object Value { get; }

            /// <summary>
            ///     Gets the reason of failure in case it has.
            /// </summary>
            public string Reason { get; }

            /// <summary>
            ///     Creates a new successful evaluation result.
            /// </summary>
            /// <param name="value">The value that the evaluation is supposed to return upon success.</param>
            public Result(object value)
            {
                IsSuccess = true;
                Reason = null;
                Value = value;
            }

            /// <summary>
            ///     Creates a new failed evaluation result.
            /// </summary>
            /// <param name="reason">The reason why the evaluation has failed.</param>
            public Result(string reason)
            {
                IsSuccess = false;
                Reason = reason;
                Value = null;
            }
        }

        /// <summary>
        ///     Gets a range of default <see cref="TypeReader"/>s.
        /// </summary>
        /// <returns>A range of <see cref="TypeReader"/>s that are defined in the library by default.</returns>
        public static TypeReader[] CreateDefaultReaders()
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
