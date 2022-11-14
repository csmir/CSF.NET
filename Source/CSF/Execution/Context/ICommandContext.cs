using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Represents an abstract context for text commands.
    /// </summary>
    public interface ICommandContext : IContext
    {
        /// <remarks>
        ///     The raw input of the command.
        /// </remarks>
        string RawInput { get; set; }

        /// <summary>
        ///     The flags present on the command input.
        /// </summary>
        IReadOnlyDictionary<string, object> Flags { get; }

        /// <summary>
        ///     The prefix for the command.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if not set.
        /// </remarks>
        IPrefix Prefix { get; }
    }
}
