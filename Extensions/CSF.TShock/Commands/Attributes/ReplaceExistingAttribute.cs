using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.TShock
{
    /// <summary>
    ///     Represents an attribute that marks if another command with matching name should be replaced on registration.
    /// </summary>
    public sealed class ReplaceExistingAttribute : Attribute
    {
        /// <summary>
        ///     Defines if a command should be replaced on registration.
        /// </summary>
        public bool ShouldReplace { get; }

        /// <summary>
        ///     Defines a new <see cref="ReplaceExistingAttribute"/> that should replace an existing registered command.
        /// </summary>
        /// <param name="replace"></param>
        public ReplaceExistingAttribute(bool replace = true)
        {
            ShouldReplace = replace;
        }
    }
}
