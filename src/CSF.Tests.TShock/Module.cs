using CSF.TShock;
using TShockAPI;

namespace CSF.Tests.TShock
{
    [RequirePermission("plugin")]
    public class Module : TSModuleBase<TSCommandContext>
    {
        [RequirePermission("test")]
        [Command("test")]
        public void Test(TSPlayer player, TSPlayer _)
        {
            Respond(player.Name);
        }
    }
}
