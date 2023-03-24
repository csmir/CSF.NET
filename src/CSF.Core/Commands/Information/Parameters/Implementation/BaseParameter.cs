using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace CSF
{
    /// <summary>
    ///     Represents a single parameter for the method.
    /// </summary>
    public sealed class BaseParameter : IParameterComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IList<Attribute> Attributes { get; }

        /// <summary>
        ///     The typereader used to populate this parameter.
        /// </summary>
        public ITypeReader TypeReader { get; }

        public BaseParameter(ParameterInfo parameterInfo, TypeReaderProvider typeReaders)
        {
            var type = parameterInfo.ParameterType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
                type = nullableType;

            Type = type;
            TypeReader = GetTypeReader(typeReaders);

            Attributes = GetAttributes(parameterInfo)
                .ToList();
            Flags = SetFlags(parameterInfo);

            Name = parameterInfo.Name;
        }

        private ITypeReader GetTypeReader(TypeReaderProvider typeReaders)
        {
            if (typeReaders.TryGetReader(Type, out var reader))
                return reader;

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

            var flags = ParameterFlags.None
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional);

            if (Attributes.Any(x => x is RemainderAttribute))
            {
                if (Type != typeof(string))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on string parameters.");

                flags = flags.WithRemainder();
            }

            return flags;
        }
        public override string ToString()
            => $"{Type.Name} {Name}";
    }
}
