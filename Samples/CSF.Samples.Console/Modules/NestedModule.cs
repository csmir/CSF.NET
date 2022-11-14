using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Samples.Console.Modules
{
    // Preconditions are allowed to exist on module & command level.
    // This precondition will return if the provided ICommandContext does not directly match CommandContext.
    [RequireContext(typeof(CommandContext))]
    public sealed class NestedModule : ModuleBase<CommandContext>
    {
        public sealed class InnerNestedModule : ModuleBase<CommandContext>
        {

        }
    }
}
