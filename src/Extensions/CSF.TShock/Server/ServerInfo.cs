using System.Collections.Generic;
using System.Linq;
using TShockAPI;

namespace CSF.TShock
{
    /// <summary>
    ///     Information about the server that this command was invoked on.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        ///     All players online at the moment of command execution.
        /// </summary>
        /// <remarks>
        ///     This value represents all players that are not <see langword="null"/>, <see cref="TSPlayer.Active"/> and a <see cref="TSPlayer.RealPlayer"/>.
        /// </remarks>
        public IEnumerable<TSPlayer> Players { get; }

        /// <summary>
        ///     The amount of players on the server at the moment of command execution.
        /// </summary>
        public int PlayerCount { get; }

        /// <summary>
        ///     The player acting as the server itself.
        /// </summary>
        public TSPlayer Console { get; }

        /// <summary>
        ///     The settings defined on the server itself.
        /// </summary>
        public TShockAPI.Configuration.TShockSettings Settings { get; }

        internal ServerInfo()
        {
            Players = TShockAPI.TShock.Players.Where(x => x is not null && x.Active && x.RealPlayer);
            PlayerCount = Players.Count();

            Console = TSPlayer.Server;

            Settings = TShockAPI.TShock.Config.Settings;
        }
    }
}
