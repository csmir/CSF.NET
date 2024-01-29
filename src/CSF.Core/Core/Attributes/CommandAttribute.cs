using CSF.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Core
{
    /// <summary>
    ///     An attribute that represents the required info to map a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        public CommandAttribute([DisallowNull] string name)
            : this(name, [])
        {

        }

        [CLSCompliant(false)]
        public CommandAttribute([DisallowNull] string name, params string[] aliases)
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
