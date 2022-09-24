using CSF.Preconditions;
using CSF.TypeReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSF.Info
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        ///     The module type.
        /// </summary>
        public Type Type { get; }

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
        }

        private IEnumerable<ParameterInfo> GetParameters(IReadOnlyDictionary<Type, ITypeReader> typeReaders, MethodInfo method)
        {
            foreach (var param in method.GetParameters())
            {
                var parserType = param.ParameterType;
                var nullableType = Nullable.GetUnderlyingType(param.ParameterType);
                var isNullable = !(nullableType is null);

                if (isNullable)
                    parserType = nullableType;

                if (typeReaders.TryGetValue(parserType, out var value))
                    yield return new ParameterInfo(param, value, isNullable);
                else
                    throw new InvalidOperationException($"No {nameof(ITypeReader)} exists for type {param.ParameterType.FullName}");
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
            foreach (var attribute in method.GetCustomAttributes(true))
            {
                if (attribute is Attribute attr)
                    yield return attr;
            }
        }
    }
}
