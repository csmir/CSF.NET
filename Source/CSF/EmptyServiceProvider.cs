using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Represents an empty service provider.
    /// </summary>
    internal class EmptyServiceProvider : IServiceProvider
    {
        /// <summary>
        ///     Represents the instance for this empty provider.
        /// </summary>
        public static EmptyServiceProvider Instance { get; } = new EmptyServiceProvider();

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
