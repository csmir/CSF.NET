using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSF
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
        ///     The constructor used to create an instance of the command type.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        ///     The method used to execute the command.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     The range of attributes present on this command.
        /// </summary>
        public IEnumerable<Attribute> Attributes { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandInfo"/> from the provided data.
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="method"></param>
        internal CommandInfo(ConstructorInfo constructor, MethodInfo method, Type type, object[] attributes, string name, string description)
        {
            IEnumerable<Attribute> GetAttributes()
            {
                foreach (var attr in attributes)
                {
                    if (attr is Attribute attribute)
                        yield return attribute;
                }
            }

            Name = name;
            Description = description;
            Attributes = GetAttributes();
            Type = type;
            Constructor = constructor;
            Method = method;
        }
    }
}
