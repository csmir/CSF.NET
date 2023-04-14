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
        public override abstract object Read(IContext context, IParameterComponent parameter, IServiceProvider services, string value);
    }

    public abstract class TypeReader
    {
        /// <inheritdoc />
        public abstract Type Type { get; }

        /// <inheritdoc />
        public abstract object Read(IContext context, IParameterComponent parameter, IServiceProvider services, string value);

        [DoesNotReturn]
        public virtual object Fail(string message = null, Exception exception = null)
            => throw new ReadException(message, exception);

        /// <summary>
        ///     Gets a range of default <see cref="TypeReader"/>s.
        /// </summary>
        /// <returns>A range of <see cref="TypeReader"/>s that are defined by default.</returns>
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
