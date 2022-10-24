using TShockAPI;

namespace CSF.TShock
{
    /// <summary>
    ///     Represents the context for TShock commands.
    /// </summary>
    /// <inheritdoc/>
    public interface ITSCommandContext : ICommandContext
    {
        /// <summary>
        ///     Determines if the command is silently executed or not.
        /// </summary>
        bool IsSilent { get; }

        /// <summary>
        ///     The player invoking this command.
        /// </summary>
        TSPlayer Player { get; }

        /// <summary>
        ///     The server this command was invoked on.
        /// </summary>
        ServerInfo Server { get; }

        /// <summary>
        ///     The TShock arguments provided for this command.
        /// </summary>
        CommandArgs CommandArguments { get; }
    }
}
