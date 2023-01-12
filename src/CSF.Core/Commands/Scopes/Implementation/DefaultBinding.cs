using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <inheritdoc/>
    public readonly struct DefaultBinding : IBinding
    {
        /// <inheritdoc/>
        public object Binding { get; }

        /// <summary>
        ///     Creates a new <see cref="DefaultBinding"/> with provided value.
        /// </summary>
        /// <param name="value"></param>
        public DefaultBinding(object value)
        {
            Binding = value;
        }
    }
}
