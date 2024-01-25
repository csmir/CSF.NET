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
        public Type ExposedType { get; }

        /// <inheritdoc/>
        public bool IsNullable { get; }

        /// <inheritdoc/>
        public bool IsOptional { get; }

        /// <inheritdoc/>
        public bool IsRemainder { get; }

        /// <inheritdoc/>
        public Attribute[] Attributes { get; }

        /// <inheritdoc/>
        public TypeReader TypeReader { get; }

        internal Parameter(ParameterInfo parameterInfo, IDictionary<Type, TypeReader> typeReaders)
        {
            var underlying = Nullable.GetUnderlyingType(parameterInfo.ParameterType);
            var attributes = parameterInfo.GetAttributes(false);

            if (underlying != null)
            {
                IsNullable = true;
                Type = underlying;
            }
            else
            {
                IsNullable = false;
                Type = parameterInfo.ParameterType;
            }

            if (parameterInfo.IsOptional)
                IsOptional = true;
            else
                IsOptional = false;

            if (attributes.Contains<RemainderAttribute>(false))
                IsRemainder = true;
            else
                IsRemainder = false;

            if (Type.IsEnum)
                TypeReader = EnumTypeReader.GetOrCreate(Type);

            else if (Type != typeof(string) && Type != typeof(object))
                TypeReader = typeReaders[Type];

            Attributes = attributes;
            ExposedType = parameterInfo.ParameterType;
            Name = parameterInfo.Name;
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{Type.Name} {Name}";
    }
}
