using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a container to activate and store typereaders.
    /// </summary>
    public sealed class TypeReaderContainer
    {
        /// <summary>
        ///     The stored typereaders.
        /// </summary>
        public IDictionary<Type, ITypeReader> Values { get; }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderContainer"/>.
        /// </summary>
        /// <param name="typeReaders">The injected typereaders to activate and store.</param>
        public TypeReaderContainer(IEnumerable<ITypeReader> typeReaders)
            => Values = typeReaders.ToDictionary(x => x.Type, x => x);
    }
}
