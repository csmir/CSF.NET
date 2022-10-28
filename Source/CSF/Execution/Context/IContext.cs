using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents an interface that supports all implementations of command context classes.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The command parameters.
        /// </summary>
        IReadOnlyList<object> Parameters { get; }

        /// <summary>
        ///     The source of this command execution.
        /// </summary>
        ISource Source { get; }
    }
}
