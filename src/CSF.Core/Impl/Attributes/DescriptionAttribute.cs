using System;

namespace CSF
{
    /// <summary>
    ///     Represents the description of a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DescriptionAttribute : Attribute
    {
        /// <summary>
        ///     The description of this parameter, command or module.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     Sets up a new <see cref="DescriptionAttribute"/> with provided value.
        /// </summary>
        /// <param name="description"></param>
        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
