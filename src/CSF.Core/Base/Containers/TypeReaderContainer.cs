using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public sealed class TypeReaderContainer
    {
        public Dictionary<Type, ITypeReader> Values { get; }

        public TypeReaderContainer(IEnumerable<ITypeReader> discoveredReaders)
        {
            Values = TypeReader.CreateDefaultReaders().Concat(discoveredReaders.ToDictionary(x => x.Type, x => x))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public ITypeReader this[Type type]
        {
            get
            {
                if (Values.ContainsKey(type))
                    return Values[type];

                throw new InvalidOperationException($"No {nameof(ITypeReader)} exists for type {type.Name}.");
            }
        }

        public TypeReaderContainer Include(ITypeReader reader)
        {
            if (Values.ContainsKey(reader.Type))
                Values[reader.Type] = reader;
            else
                Values.Add(reader.Type, reader);
            return this;
        }

        public TypeReaderContainer Exclude(Type type)
        {
            Values.Remove(type);
            return this;
        }
    }
}
