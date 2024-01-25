using CSF.Helpers;
using CSF.TypeReaders;
using System.Reflection;

namespace CSF.Reflection
{
    /// <summary>
    ///     Represents a complex parameter, containing a number of its own parameters.
    /// </summary>
    public class ComplexArgumentInfo : IArgument, IArgumentBucket
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
        public IArgument[] Parameters { get; }

        /// <inheritdoc/>
        public bool HasParameters { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int MaxLength { get; }

        /// <inheritdoc/>
        public TypeReader TypeReader { get; }

        /// <summary>
        ///     Gets the constructor that constructs this complex parameter.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        internal ComplexArgumentInfo(ParameterInfo parameterInfo, IDictionary<Type, TypeReader> typeReaders)
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

            var constructor = Type.GetConstructors()[0];
            var parameters = constructor.GetParameters(typeReaders);

            if (parameters.Length > 0)
            {
                ThrowHelpers.InvalidOp("Complex types are expected to have at least 1 constructor parameter.");
            }

            var (minLength, maxLength) = parameters.GetLength();

            IsRemainder = false;
            TypeReader = null;

            MinLength = minLength;
            MaxLength = maxLength;

            Constructor = constructor;
            Parameters = parameters;
            HasParameters = parameters.Length > 0;

            Attributes = attributes;

            ExposedType = parameterInfo.ParameterType;
            Name = parameterInfo.Name;
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{Type.Name} ({string.Join<IArgument>(", ", Parameters)}) {Name}";
    }
}
