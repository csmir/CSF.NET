using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents an injectable service.
    /// </summary>
    public sealed class DependencyParameter : IParameterComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IList<Attribute> Attributes { get; }

        public DependencyParameter(ParameterInfo parameterInfo)
        {
            var type = parameterInfo.ParameterType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
                type = nullableType;

            Type = type;

            Attributes = GetAttributes(parameterInfo)
                .ToList();
            Flags = SetFlags(parameterInfo);

            Name = parameterInfo.Name;
        }

        private IEnumerable<Attribute> GetAttributes(ParameterInfo paramInfo)
        {
            foreach (var obj in paramInfo.GetCustomAttributes(false))
                if (obj is Attribute attribute)
                    yield return attribute;
        }

        private ParameterFlags SetFlags(System.Reflection.ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var isNullable = Nullable.GetUnderlyingType(type) != null;

            var flags = ParameterFlags.None
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional);

            return flags;
        }

        public override string ToString()
            => $"{Type.Name} {Name}";
    }
}
