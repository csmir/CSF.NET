using System;

namespace CSF
{
    /// <summary>
    ///     Represents an attribute that describes a command parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        ///     The parameter name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The parameter description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="ParameterAttribute"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="isRequired"></param>
        public ParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
