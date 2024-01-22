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
        public abstract ValueTask<CommandManager.ReadResult> EvaluateAsync(ICommandContext context, IParameterComponent parameter, object value);

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
