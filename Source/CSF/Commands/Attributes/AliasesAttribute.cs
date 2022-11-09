using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     An attribute that represents multiple command aliases for quick execution.
    /// </summary>
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
            Aliases = aliases.Split('|');
        }
    }
}
