using CSF.TShock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
