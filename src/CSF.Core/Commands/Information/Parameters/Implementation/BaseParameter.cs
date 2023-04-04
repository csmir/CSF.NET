using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public BaseParameter(ParameterInfo parameterInfo)
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
