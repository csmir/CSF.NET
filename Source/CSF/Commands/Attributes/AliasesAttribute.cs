using System;
using System.Xml.Linq;

namespace CSF
{
    /// <summary>
    ///     An attribute that represents multiple command aliases for quick execution.
    /// </summary>
    [Obsolete("This item will be removed in the next major update.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AliasesAttribute : Attribute
    {
        /// <summary>
        ///     The aliases for the target command.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="AliasesAttribute"/>.
        /// </summary>
        /// <param name="aliases">The aliases for this command.</param>
        [CLSCompliant(false)]
        public AliasesAttribute(params string[] aliases)
        {
            foreach (var alias in aliases)
                if (string.IsNullOrWhiteSpace(alias))
                    throw new ArgumentNullException(nameof(alias), "Alias cannot be null or empty.");

            Aliases = aliases;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AliasesAttribute"/>.
        /// </summary>
        /// <remarks>
        ///     This overload supports multiple prefixes only by splitting the string. Use '|' to declare new prefixes.
        ///     <br/>
        ///     If CLS compliancy is not of concern, use <see cref="AliasesAttribute(string[])"/> instead.
        /// </remarks>
        /// <param name="aliases">The aliases for this command.</param>
        [CLSCompliant(true)]
        public AliasesAttribute(string aliases)
        {
            if (string.IsNullOrWhiteSpace(aliases))
                throw new ArgumentNullException(nameof(aliases), "Aliases cannot be null or empty.");

            Aliases = aliases.Split('|');
        }
    }
}
