using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents an abstract context for text commands.
    /// </summary>
    public interface IConsoleContext : IContext
    {
        /// <remarks>
        ///     The raw input of the command.
        /// </remarks>
        public string RawInput { get; set; }

        /// <summary>
        ///     The flags present on the command input.
        /// </summary>
        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        /// <summary>
        ///     The prefix for the command.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if not set.
        /// </remarks>
        public string Prefix { get; }
    }
}
