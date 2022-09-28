using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public sealed class CommandInfo
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        ///     The list of parameters for this command.
        /// </summary>
        public IReadOnlyCollection<ParameterInfo> Parameters { get; }

        /// <summary>
        ///     The range of attributes present on this command.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The range of precondition attributes present on this command.
        /// </summary>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandInfo"/> from the provided data.
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="method"></param>
        internal CommandInfo(IReadOnlyDictionary<Type, ITypeReader> typeReaders, ModuleInfo module, MethodInfo method, string[] aliases)
        {
            Name = aliases[0];
            Aliases = aliases;

            Method = method;
            Module = module;

            Attributes = Module.Attributes.Concat(GetAttributes(Method)).ToList();
            Preconditions = module.Preconditions.Concat(GetPreconditions(Attributes)).ToList();
            Parameters = GetParameters(typeReaders, Method).ToList();

            var remainderParameters = Parameters.Where(x => x.IsRemainder);
            if (remainderParameters.Any())
            {
                if (remainderParameters.Count() > 1)
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} cannot exist on multiple parameters at once.");

                if (!Parameters.Last().IsRemainder)
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");
            }
        }

        private IEnumerable<ParameterInfo> GetParameters(IReadOnlyDictionary<Type, ITypeReader> typeReaders, MethodInfo method)
        {
            var parameters = method.GetParameters();

            foreach (var param in method.GetParameters())
            {
                var paramType = param.ParameterType;
                var nullableType = Nullable.GetUnderlyingType(param.ParameterType);
                var isNullable = !(nullableType is null);

                if (isNullable)
                    paramType = nullableType;

                if (paramType == typeof(string))
                    yield return new ParameterInfo(param, null, isNullable);
                else if (typeReaders.TryGetValue(paramType, out var value))
                    yield return new ParameterInfo(param, value, isNullable);
                else
                    throw new InvalidOperationException($"No {nameof(ITypeReader)} exists for type {paramType}");
            }
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions(IReadOnlyCollection<Attribute> attributes)
        {
            foreach (var attr in attributes)
            {
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
            }
        }

        private IEnumerable<Attribute> GetAttributes(MethodInfo method)
        {
            foreach (var attribute in method.GetCustomAttributes(false))
            {
                if (attribute is Attribute attr)
                    yield return attr;
            }
        }
    }
}
