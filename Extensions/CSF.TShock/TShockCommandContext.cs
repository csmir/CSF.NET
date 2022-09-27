using System;
using System.Collections.Generic;
using System.Text;
using TShockAPI;

namespace CSF.TShock
{
    /// <summary>
    ///     A command context providing TShock command info.
    /// </summary>
    public class TShockCommandContext : ICommandContext
    {
        public string Name { get; }

        public string RawInput { get; }

        public List<string> Parameters { get; }

        /// <summary>
        ///     Determines if the command is silently executed or not.
        /// </summary>
        public bool IsSilent { get; }

        /// <summary>
        ///     The player invoking this command.
        /// </summary>
        public TSPlayer Player { get; }

        /// <summary>
        ///     The TShock arguments provided for this command.
        /// </summary>
        public CommandArgs CommandArguments { get; }

        /// <summary>
        ///     Creates a new CommandContext from the provided 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="rawInput"></param>
        public TShockCommandContext(CommandArgs args, string rawInput) 
        {
            CommandArguments = args;
            RawInput = rawInput;
            Parameters = args.Parameters;
            Player = args.Player;
            IsSilent = args.Silent;

            // Skip prefix & get first occurence
            Name = args.Message.Substring(1).Split(' ')[0];
        }
    }
}
