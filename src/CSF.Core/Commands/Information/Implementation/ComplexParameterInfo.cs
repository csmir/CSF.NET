using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    public class ComplexParameterInfo : IParameterComponent, IParameterContainer
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public ParameterFlags Flags { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameterComponent> Parameters { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int OptimalLength { get; }

        /// <summary>
        ///     The complexParam constructor for complexParam parameter types.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        internal ComplexParameterInfo(System.Reflection.ParameterInfo parameterInfo, TypeReaderProvider typeReaders)
        {
            var type = parameterInfo.GetType();

            Type = type;
            Flags = SetFlags(parameterInfo);

            Constructor = new ConstructorInfo(Type);

            Parameters = GetParameters(typeReaders).ToList();
            Attributes = GetAttributes(parameterInfo).ToList();

            (int min, int nom) = GetLength();

            MinLength = min;
            OptimalLength = nom;
        }

        private (int, int) GetLength()
        {
            var minLength = 0;
            var nomLength = 0;

            foreach (var parameter in Parameters)
            {
                if (parameter is ComplexParameterInfo complexParam)
                {
                    nomLength += complexParam.OptimalLength;
                    minLength += complexParam.MinLength;
                }

                if (parameter is ParameterInfo defaultParam)
                {
                    nomLength++;
                    if (!defaultParam.Flags.HasFlag(ParameterFlags.IsOptional))
                        minLength++;
                }
            }

            return (minLength, nomLength);
        }

        private ParameterFlags SetFlags(System.Reflection.ParameterInfo paramInfo)
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
                if (parameter.GetCustomAttributes(true).Any(x => x is ComplexAttribute))
                    yield return new ComplexParameterInfo(parameter, typeReaders);
                else
                    yield return new ParameterInfo(parameter, typeReaders);
            }
        }

        private IEnumerable<Attribute> GetAttributes(System.Reflection.ParameterInfo paramInfo)
        {
            foreach (var attribute in paramInfo.GetCustomAttributes(false))
                if (attribute is Attribute attr)
                    yield return attr;
        }
    }
}
