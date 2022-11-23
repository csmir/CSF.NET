using CSF.TShock;
using System.Linq;

namespace CSF.Samples.TShock
{
    // This node sets a top level permission, i.e. 'csf....'
    [RequirePermission("csf")]
    public sealed class Commands : TShockCommandBase
    {
        // Defines a command with the name 'who', adding a description that the registrator will use as TShock's command HELPTEXT.
        [Command("who")]
        [Description("Gets all players on the server.")]
        // This node will turn into 'csf.who.index'
        [RequirePermission("who")]
        public void Who(bool getIndex = false) // optional parameter getIndex, if someone would execute /who true it will resolve successfully.
        {
            var players = TShockAPI.TShock.Players.Where(x => x != null && x.RealPlayer && x.Active);

            var stringified = players.Select(x => $"{x.Name}{(getIndex ? $" (i: {x.Index})" : "")}");

            RespondInformation($"All online players ({players.Count()}/{TShockAPI.TShock.Config.Settings.MaxSlots}): \n{stringified}");
        }
    }
}
