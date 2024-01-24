using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     Represents a command group, functioning much like subcommands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GroupAttribute : Attribute
    {
        /// <summary>
        ///     Represents the name of a group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The aliases of this group.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Creates a new <see cref="GroupAttribute"/> with defined name.
        /// </summary>
        /// <param name="name"></param>
        public GroupAttribute(string name)
            : this(name, [])
        {

        }

        /// <summary>
        ///     Creates a new <see cref="GroupAttribute"/> with defined name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="aliases"></param>
        [CLSCompliant(false)]
        public GroupAttribute([DisallowNull] string name, params string[] aliases)
        {
            if (string.IsNullOrWhiteSpace(name))
                ThrowHelpers.InvalidArg(name);

            var arr = new string[aliases.Length + 1];

            arr[0] = name;

            Array.Copy(aliases, 0, arr, 1, aliases.Length);

            Name = name;
            Aliases = arr;
        }
    }
}
