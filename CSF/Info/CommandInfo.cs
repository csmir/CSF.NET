using System;
using System.Collections.Generic;
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
        ///     The command description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandInfo"/> from the provided data.
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="method"></param>
        internal CommandInfo(ConstructorInfo constructor, MethodInfo method, Type type, object[] attributes, string name, string description)
        {
            Name = name;
            Description = description;
            Module = new ModuleInfo(attributes, constructor);
            Method = method;
            Type = type;
        }
    }
}
