﻿using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     Represents a generic <see cref="TypeReader{T}"/> to use for parsing provided types into the targetted type.
    /// </summary>
    /// <typeparam name="T">The targetted type for this typereader.</typeparam>
    public abstract class TypeReader<T> : ITypeReader
    {
        /// <inheritdoc />
        public Type Type { get; } = typeof(T);

        /// <inheritdoc />
        public abstract object Read(IContext context, IParameterComponent parameter, IServiceProvider services, string value);

        [DoesNotReturn]
        public virtual object Fail(string message = null, Exception exception = null)
            => throw new ReadException(message, exception);
    }

    public static class TypeReader
    {
        /// <summary>
        ///     Gets a range of default <see cref="ITypeReader"/>'s.
        /// </summary>
        /// <returns>A range of <see cref="ITypeReader"/>'s that are defined by default.</returns>
        public static IEnumerable<ITypeReader> CreateDefaultReaders()
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