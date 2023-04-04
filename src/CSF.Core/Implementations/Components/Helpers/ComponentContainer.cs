using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a container to activate and store components.
    /// </summary>
    public sealed class ComponentContainer
    {
        /// <summary>
        ///     The stored components.
        /// </summary>
        public IEnumerable<IConditionalComponent> Values { get; }

        /// <summary>
        ///     Creates a new <see cref="ComponentContainer"/>.
        /// </summary>
        /// <param name="types">The types to activate and store.</param>
        public ComponentContainer(IEnumerable<Type> types)
            => Values = types.SelectMany(x => new Module(x).Components);
    }
}
