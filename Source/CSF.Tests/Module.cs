namespace CSF.Tests
{
    public class Module : ModuleBase<CommandContext>
    {
        [Command("loglevel")]
        public IResult LogLevel(LogLevel level)
        {
            Logger.LogLevel = level;

            return Success("Success");
        }

        [Command("test")]
        public IResult Test(int help)
        {
            return Success(help.ToString());
        }

        [Command("test")]
        public IResult Test(long helping, long helping2)
        {
            return Success("Success");
        }

        [Command("test")]
        [ErrorOverload]
        public IResult Test()
        {
            return Success("Success");
        }

        [Command("group")]
        [ErrorOverload]
        public void Group()
        {

        }

        [Group("group", "gr")]
        public class InnerModule : ModuleBase<CommandContext>
        {
            [Group("group")]
            public class InnerInnerModule : ModuleBase<CommandContext>
            { 
                [Group("group")]
                public class InnerInnerInnerModule : ModuleBase<CommandContext>
                {
                    [Command("subcommand")]
                    public IResult MostInnerTest()
                    {
                        return Success("Success");
                    }
                }
            }
        }
    }
}
