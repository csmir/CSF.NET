using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents a complex parameter, containing a number of its own parameters.
    /// </summary>
    public class ComplexParameter : IParameterComponent, IParameterContainer
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IList<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IList<IParameterComponent> Parameters { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int MaxLength { get; }

        /// <summary>
        ///     The complexParam constructor for complexParam parameter types.
        /// </summary>
        public Constructor Constructor { get; }

        internal ComplexParameter(ParameterInfo parameterInfo, TypeReaderProvider typeReaders)
        {
            var type = parameterInfo.ParameterType;

            Type = type;

            Attributes = GetAttributes(parameterInfo)
                .ToList();
            Parameters = GetParameters(typeReaders)
                .ToList();

            Flags = SetFlags(parameterInfo);

            Constructor = new Constructor(Type);

            (int min, int max) = GetLength();

            MinLength = min;
            MaxLength = max;
        }

        private (int, int) GetLength()
        {
            var minLength = 0;
            var maxLength = 0;

            foreach (var parameter in Parameters)
            {
                if (parameter is ComplexParameter complexParam)
                {
                    maxLength += complexParam.MaxLength;
                    minLength += complexParam.MinLength;
                }

                if (parameter is BaseParameter defaultParam)
                {
                    maxLength++;
                    if (!defaultParam.Flags.HasFlag(ParameterFlags.IsOptional))
                        minLength++;
                }
            }

            return (minLength, maxLength);
        }

        private ParameterFlags SetFlags(ParameterInfo paramInfo)
        {
            var type = paramInfo.ParameterType;
            var isNullable = Nullable.GetUnderlyingType(type) != null;

            var flags = ParameterFlags.Default
                .WithNullable(isNullable)
                .WithOptional(paramInfo.IsOptional);

            if (Attributes.Any(x => x is RemainderAttribute))
                throw new InvalidOperationException("Remainder attributes cannot be set on complexParam parameters.");

            return flags;
        }

        private IEnumerable<IParameterComponent> GetParameters(TypeReaderProvider typeReaders)
        {
            var parameters = Constructor.EntryPoint.GetParameters();

            if (!parameters.Any())
                throw new InvalidOperationException("Complex parameters require at least constructor parameter to be defined.");

            foreach (var parameter in Constructor.EntryPoint.GetParameters())
            {
                if (parameter.GetCustomAttributes().Any(x => x is ComplexAttribute))
                    yield return new ComplexParameter(parameter, typeReaders);
                else
                    yield return new BaseParameter(parameter, typeReaders);
            }
        }

        private IEnumerable<Attribute> GetAttributes(ParameterInfo paramInfo)
        {
            foreach (var attribute in paramInfo.GetCustomAttributes(false))
                if (attribute is Attribute attr)
                    yield return attr;
        }
    }
}
