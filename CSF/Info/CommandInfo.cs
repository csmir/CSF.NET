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
        internal CommandInfo(IReadOnlyDictionary<Type, ITypeReader> typeReaders, ModuleInfo module, MethodInfo method, string name)
        {
            IEnumerable<ParameterInfo> GetParameters()
            {
                foreach (var param in method.GetParameters())
                {
                    if (typeReaders.TryGetValue(param.ParameterType, out var value))
                        yield return new ParameterInfo(param, value);
                    else
                        throw new InvalidOperationException($"No {nameof(ITypeReader)} exists for type {param.ParameterType.FullName}");
                }
            }

            IEnumerable<Attribute> GetAttributes()
            {
                foreach (var attribute in method.GetCustomAttributes(true))
                {
                    if (attribute is Attribute attr)
                        yield return attr;
                }
            }

            IEnumerable<PreconditionAttribute> GetPreconditions()
            {
                foreach (var attr in Attributes)
                {
                    if (attr is PreconditionAttribute precondition)
                        yield return precondition;
                }
            }

            Name = name;
            Module = module;
            Attributes = Module.Attributes.Concat(GetAttributes()).ToList();
            Preconditions = module.Preconditions.Concat(GetPreconditions()).ToList();
            Parameters = GetParameters().ToList();
            Method = method;
        }
    }
}
