using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public sealed class TypeReaderContainer
    {
        public IDictionary<Type, ITypeReader> Values { get; }

        public TypeReaderContainer(IEnumerable<ITypeReader> typeReaders)
            => Values = typeReaders.ToDictionary(x => x.Type, x => x);
    }
}
