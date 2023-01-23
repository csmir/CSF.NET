using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public sealed class Command : IConditionalComponent, IParameterContainer
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameterComponent> Parameters { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int MaxLength { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public Module Module { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     Represents if the command is an error overload.
        /// </summary>
        public bool IsErrorOverload { get; }

        internal Command(TypeReaderProvider typeReaders, Module module, MethodInfo method, string[] aliases)
        {
            Method = method;
            Module = module;

            Attributes = module.Attributes.Concat(GetAttributes()).ToList();
            Preconditions = module.Preconditions.Concat(GetPreconditions()).ToList();

            Parameters = GetParameters(typeReaders).ToList();

            var remainderParameters = Parameters.Where(x => x.Flags.HasFlag(ParameterFlags.IsRemainder));
            if (remainderParameters.Any())
            {
                if (remainderParameters.Count() > 1)
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} cannot exist on multiple parameters at once.");

                if (!Parameters.Last().Flags.HasFlag(ParameterFlags.IsRemainder))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");
            }

            if (Attributes.Any(x => x is ErrorOverloadAttribute))
            {
                if (Parameters.Any())
                    throw new InvalidOperationException($"{nameof(ErrorOverloadAttribute)} cannot exist on a method with parameters.");

                IsErrorOverload = true;
            }

            Name = aliases[0];
            Aliases = aliases;

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

        private IEnumerable<IParameterComponent> GetParameters(TypeReaderProvider typeReaders)
        {
            foreach (var parameter in Method.GetParameters())
            {
                if (parameter.GetCustomAttributes(true).Any(x => x is ComplexAttribute))
                    yield return new ComplexParameter(parameter, typeReaders);
                else
                    yield return new BaseParameter(parameter, typeReaders);
            }
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions()
        {
            foreach (var attr in Attributes)
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attribute in Method.GetCustomAttributes(true))
                if (attribute is Attribute attr)
                    yield return attr;
        }
    }
}
