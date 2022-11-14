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
            [Command("subcommand")]
            public IResult MostInnerTest()
            {
                return Success("Success");
            }

            [Command("subcommand")]
            public IResult MostInnerTest(int i)
            {
                return Info("{0}", i);
            }

            [Group("group")]
            public class InnerInnerModule : ModuleBase<CommandContext>
            {
                [Command("subcommand")]
                public IResult MostInnerTest()
                {
                    return Success("Success");
                }

                [Command("subcommand")]
                public IResult MostInnerTest([Remainder] string remainder)
                {
                    return Info(remainder);
                }
            }
        }
    }
}
