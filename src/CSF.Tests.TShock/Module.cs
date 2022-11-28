using CSF.TShock;

namespace CSF.Tests.TShock
{
    [RequirePermission("plugin")]
    public class Module : TSModuleBase<TSCommandContext>
    {
        [RequirePermission("test")]
        [Command("test", "t")]
        public void Test()
            => Success("tested!");
    }
}
