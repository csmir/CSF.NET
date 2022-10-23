using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace CSF.TShock
{
    internal class PlayerReader : TypeReader<Player>
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo info, string value, IServiceProvider provider)
        {
            var players = TSPlayer.FindByNameOrID(value);

            if (!players.Any())
                return Task.FromResult(TypeReaderResult.FromError("No player found."));

            else if (players.Count > 1)
                return Task.FromResult(TypeReaderResult.FromError($"Multiple players found: \n{string.Join(", ", players.Select(x => x.Name))}"));

            else
                return Task.FromResult(TypeReaderResult.FromSuccess(players[0].TPlayer));
        }
    }
}
