using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a default interface for the <see cref="CommandContext"/> class.
    /// </summary>
    public interface ICommandContext
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
    }
}
