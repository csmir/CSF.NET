using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents a single parameter for the method.
    /// </summary>
    public sealed class Parameter : IParameterComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The typereader used to populate this parameter.
        /// </summary>
        public ITypeReader TypeReader { get; }

        internal Parameter(ParameterInfo paramInfo, TypReaderProvider dictionary)
        {
            var type = paramInfo.ParameterType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
                type = nullableType;

            Type = type;
            TypeReader = GetTypeReader(dictionary);

            Attributes = GetAttributes(paramInfo).ToList();
            Flags = SetFlags(paramInfo);

            Name = paramInfo.Name;
        }

        private ITypeReader GetTypeReader(TypReaderProvider dictionary)
        {
            if (dictionary.TryGetReader(Type, out var reader))
                return reader;

            else
                throw new InvalidOperationException($"No {nameof(ITypeReader)} exists for type {Type.Name}.");
        }

        private IEnumerable<Attribute> GetAttributes(ParameterInfo paramInfo)
        {
            foreach (var attribute in paramInfo.GetCustomAttributes(false))
                if (attribute is Attribute attr)
                    yield return attr;
        }

        private ParameterFlags SetFlags(ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var isNullable = Nullable.GetUnderlyingType(type) != null;

            var flags = ParameterFlags.Default
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional);

            if (Attributes.Any(x => x is RemainderAttribute))
            {
                if (Type != typeof(string))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on string parameters.");

                flags.WithRemainder();
            }

            return flags;
        }
    }
}
