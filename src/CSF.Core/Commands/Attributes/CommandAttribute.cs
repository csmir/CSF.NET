using System;
using System.Linq;

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
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Sets up a new command attribute with the provided name.
        /// </summary>
        /// <param name="name"></param>
        public CommandAttribute(string name)
            : this(name, Array.Empty<string>())
        {

        }

        /// <summary>
        ///     Sets up a new command attribute with the provided name and aliases.
        /// </summary>
        /// <param name="name"></param>
        [CLSCompliant(false)]
        public CommandAttribute(string name, params string[] aliases)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

            foreach (var alias in aliases)
                if (string.IsNullOrWhiteSpace(alias))
                    throw new ArgumentNullException(nameof(alias), "Alias cannot be null or empty.");

            Name = name;

            Aliases = new string[] { Name }.Concat(aliases).ToArray();
        }
    }
}
