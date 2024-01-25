using CSF.Helpers;
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
        public GroupAttribute([DisallowNull] string name)
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
                ThrowHelpers.ArgMissing(name);

            var arr = new string[aliases.Length + 1];
            for (int i = 0; i < aliases.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(aliases[i]))
                    ThrowHelpers.ArgMissing(aliases);

                if (arr.Contains(aliases[i]))
                    ThrowHelpers.RangeDuplicate(aliases);

                arr[i + 1] = aliases[i];
            }

            if (arr.Contains(name))
                ThrowHelpers.RangeDuplicate(aliases);

            arr[0] = name;

            Name = name;
            Aliases = arr;
        }
    }
}
