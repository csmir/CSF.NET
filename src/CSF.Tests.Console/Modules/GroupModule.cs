namespace CSF.Tests.Console.Modules
{
    public class GroupModule : ModuleBase<CommandContext>
    {
        public GroupModule()
        {

        }

        [Command("command")]
        public void Command()
        {
            Info("Wow, this is working!");
        }

        [Group("subcommand")]
        public class InnerModule : ModuleBase<CommandContext>
        {
            [Command("command")]
            [ErrorOverload]
            public void Command()
            {
                Success("Success (overload)");
            }

            [Command("command")]
            public void Command(int i)
            {
                Success($"Success: {i}");
            }

            [Group("subsubcommand")]
            public class InnerInnerModule : ModuleBase<CommandContext>
            {
                [Command("command")]
                [ErrorOverload]
                public void Command()
                {
                    Success("Success (overload)");
                }

                [Command("command")]
                public void Command(int i)
                {
                    Success($"Success: {i}");
                }
            }
        }
    }
}
