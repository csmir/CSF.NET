using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents an interface that supports all implementations of command context classes.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        ///     The name of the command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The raw input of the command.
        /// </summary>
        string RawInput { get; }

        /// <summary>
        ///     The command parameters.
        /// </summary>
        List<string> Parameters { get; }

        /// <summary>
        ///     The source of this command execution.
        /// </summary>
        ISource Source { get; }
    }
}
