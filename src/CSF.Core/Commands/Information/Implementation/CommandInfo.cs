using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public sealed class CommandInfo : IConditionalComponent, IParameterContainer
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
        public int OptimalLength { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     Represents if the command is an error overload.
        /// </summary>
        public bool IsErrorOverload { get; }

        internal CommandInfo(TypeReaderProvider typeReaders, ModuleInfo module, MethodInfo method, string[] aliases)
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

            (int min, int nom) = GetLength();

            MinLength = min;
            OptimalLength = nom;
        }

        private (int, int) GetLength()
        {
            var minLength = 0;
            var nomLength = 0;
            bool maxOut = false;

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

                    if (defaultParam.Flags.HasFlag(ParameterFlags.IsRemainder))
                        maxOut = true;
                }
            }

            if (maxOut)
                nomLength = int.MaxValue;

            return (minLength, nomLength);
        }

        private IEnumerable<IParameterComponent> GetParameters(TypeReaderProvider typeReaders)
        {
            foreach (var parameter in Method.GetParameters())
            {
                if (parameter.GetCustomAttributes(true).Any(x => x is ComplexAttribute))
                    yield return new ComplexParameterInfo(parameter, typeReaders);
                else
                    yield return new ParameterInfo(parameter, typeReaders);
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
