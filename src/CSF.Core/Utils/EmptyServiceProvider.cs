using System;

namespace CSF
{
    /// <summary>
    ///         Represents an empty <see cref="IServiceProvider"/>. Requesting a service from this type will return <see langword="null"/>.
    /// </summary>
    public sealed class EmptyServiceProvider : IServiceProvider
    {
        /// <summary>
        ///     A static instance used to request the necessary <see cref="IServiceProvider"/> value.
        /// </summary>
        public static EmptyServiceProvider Instance { get; } = new EmptyServiceProvider();

        /// <inheritdoc/>
        /// <remarks>
        ///     Will always be <see langword="null"/>.
        /// </remarks>
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
