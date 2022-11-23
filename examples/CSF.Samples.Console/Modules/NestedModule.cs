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
        [Command("nested")]
        // Erroroverload will be the default resolver if the other matches could not be found thanks to an invalid parameter length.
        [ErrorOverload]
        public IResult Command()
        {
            return Success("Success!");
        }

        // A subcommand group
        [Group("nested")]
        public sealed class InnerNestedModule : ModuleBase<CommandContext>
        {
            [Command("inner")]
            [ErrorOverload]
            public IResult Command()
            {
                return Success("Success!");
            }
        }
    }
}
