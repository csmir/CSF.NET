using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Tests.Console.Modules
{
    public sealed class AsyncModule : ModuleBase<CommandContext>
    {
        [Command("delayed", "delay")]
        public async Task AsyncRunDelayed()
        {
            await Task.Delay(5000);

            Respond("Success. (Delayed).");
        }

        [Command("direct", "dir")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AsyncRunDirect()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            => Respond("Success. (Direct).");
    }
}
