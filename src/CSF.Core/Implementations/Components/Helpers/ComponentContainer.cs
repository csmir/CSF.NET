using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public sealed class ComponentContainer
    {
        public IEnumerable<IConditionalComponent> Values { get; }

        public ComponentContainer(IEnumerable<Type> types)
            => Values = types.SelectMany(x => new Module(x).Components);
    }
}
