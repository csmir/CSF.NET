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
        // This node sets a lower level permission, and because the top level is 'csf', it will append to become 'csf.who'
        [RequirePermission("who")]
        public void Who()
            => Who(false); // redirect to another command that does the same, but has a different entry point & parameter list.

        [Command("who")]
        [Description("Gets all players on the server.")]
        // This node will turn into 'csf.who.index'
        [RequirePermission("who.index")]
        public void Who(bool getIndex)
        {
            var players = TShockAPI.TShock.Players.Where(x => x != null && x.RealPlayer && x.Active);

            var stringified = players.Select(x => $"{x.Name}{(getIndex ? $" (i: {x.Index})" : "")}");

            RespondInformation($"All online players ({players.Count()}/{TShockAPI.TShock.Config.Settings.MaxSlots}): \n{stringified}");
        }
    }
}
