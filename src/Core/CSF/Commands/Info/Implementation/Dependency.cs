using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents an injectable service.
    /// </summary>
    public sealed class Dependency : IParameterComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        internal Dependency(ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var nullableType = Nullable.GetUnderlyingType(type);

            if (nullableType != null)
                type = nullableType;

            Type = type;

            Attributes = GetAttributes(paramInfo).ToList();
            Flags = SetFlags(paramInfo);

            Name = paramInfo.Name;
        }

        private IEnumerable<Attribute> GetAttributes(ParameterInfo paramInfo)
        {
            foreach (var obj in paramInfo.GetCustomAttributes(false))
                if (obj is Attribute attribute)
                    yield return attribute;
        }

        private ParameterFlags SetFlags(ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var isNullable = Nullable.GetUnderlyingType(type) != null;

            var isInjectable = true;

            foreach (var attribute in Attributes)
                if (attribute is DontInjectAttribute)
                    isInjectable = false;

            var flags = ParameterFlags.Default
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional)
                .WithInjectable(isInjectable);

            return flags;
        }
    }
}
