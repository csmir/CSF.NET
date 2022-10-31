using System.Collections.Generic;
using TShockAPI;

namespace CSF.TShock
{
    /// <summary>
    ///     A command context providing TShock command info.
    /// </summary>
    public class TSCommandContext : ITSCommandContext
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string RawInput { get; }

        /// <inheritdoc/>
        public IReadOnlyList<object> Parameters { get; }

        /// <inheritdoc/>
        public bool IsSilent { get; }

        /// <inheritdoc/>
        public TSPlayer Player { get; }

        /// <inheritdoc/>
        public ServerInfo Server { get; }

        /// <inheritdoc/>
        public CommandArgs CommandArguments { get; }

        /// <summary>
        ///     Creates a new CommandContext from the provided 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rawInput"></param>
        public TSCommandContext(CommandArgs args, string rawInput)
        {
            CommandArguments = args;
            RawInput = rawInput;
            Parameters = args.Parameters;
            Player = args.Player;
            IsSilent = args.Silent;

            // Skip prefix & get first occurence
            Name = args.Message[1..].Split(' ')[0];
        }
    }
}
