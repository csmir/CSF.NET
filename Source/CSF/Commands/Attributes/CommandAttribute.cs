using System;

namespace CSF
{
    /// <summary>
    ///     An attribute that represents the required info to map a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Sets up a new command attribute with the provided name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
