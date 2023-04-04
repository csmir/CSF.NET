using System;
using System.Threading;
using System.Threading.Tasks;

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
        public abstract ValueTask<TypeReaderResult> ReadAsync(IContext context, BaseParameter info, object value, CancellationToken cancellationToken);

        /// <summary>
        ///     Returns the read operation with success.
        /// </summary>
        /// <param name="value">The value to populate the target command with.</param>
        /// <returns>A <see cref="TypeReaderResult"/> from the provided result.</returns>
        public virtual TypeReaderResult Success(T value)
            => TypeReaderResult.FromSuccess(value);

        /// <summary>
        ///     Returns the read operation with an error.
        /// </summary>
        /// <param name="errorMessage">The error message that reports the error.</param>
        /// <param name="exception">The occurred exception if applicable.</param>
        /// <returns>A <see cref="TypeReaderResult"/> from the provided error.</returns>
        public virtual TypeReaderResult Error(string errorMessage, Exception exception = null)
            => TypeReaderResult.FromError(errorMessage, exception);
    }
}
