using System;

namespace CSF.Tests
{
    public class GroupModule : ModuleBase<CommandContext>
    {
        public GroupModule()
        {

        }

        [Command("command")]
        public void MyCommand()
        {
            Respond("Wow, this is working!");
        }

        [Group("subcommand")]
        public class InnerModule : ModuleBase<CommandContext>
        {
            [Command("command")]
            [ErrorOverload]
            public void MyCommand()
            {
                Respond("Success (overload)");
            }

            [Command("command")]
            public void MyCommand(int i)
            {
                Respond($"Success: {i}");
            }

            [Group("subsubcommand")]
            public class InnerInnerModule : ModuleBase<CommandContext>
            {
                [Command("command")]
                [ErrorOverload]
                public void MyCommand()
                {
                    Respond("Success (overload)");
                }

                [Command("command")]
                public void MyCommand(int i)
                {
                    Respond($"Success: {i}");
                }
            }
        }
    }
}
