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
        public string Name { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<object> Parameters { get; set; }

        /// <summary>
        ///     Represents the raw command input.
        /// </summary>
        public string RawInput { get; set; }

        /// <inheritdoc/>
        public bool IsSilent { get; }

        /// <inheritdoc/>
        public TSPlayer Player { get; }

        /// <inheritdoc/>
        public ServerInfo Server { get; }

        /// <inheritdoc/>
        public CommandArgs CommandArguments { get; }

        /// <inheritdoc/>
        public IPrefix Prefix { get; }

        /// <summary>
        ///     Creates a new CommandContext from the provided 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rawInput"></param>
        public TSCommandContext(CommandArgs args, string rawInput, IPrefix prefix)
        {
            Prefix = prefix;

            CommandArguments = args;
            RawInput = rawInput;
            Parameters = args.Parameters;
            Player = args.Player;
            IsSilent = args.Silent;

            Name = rawInput.Split(' ')[0];
        }
    }
}
