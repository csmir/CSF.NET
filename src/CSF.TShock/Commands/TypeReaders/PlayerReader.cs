using Terraria;
using TShockAPI;

namespace CSF.TShock
{
    public sealed class PlayerReader : TypeReader<Player>
    {
        public override Task<TypeReaderResult> ReadAsync(IContext context, ParameterInfo info, object value, IServiceProvider provider)
        {
            var players = TSPlayer.FindByNameOrID(value.ToString());

            if (!players.Any())
                return Task.FromResult(TypeReaderResult.FromError("No player found."));

            else if (players.Count > 1)
                return Task.FromResult(TypeReaderResult.FromError($"Multiple players found: \n{string.Join(", ", players.Select(x => x.Name))}"));

            else
                return Task.FromResult(TypeReaderResult.FromSuccess(players[0].TPlayer));
        }
    }
}
