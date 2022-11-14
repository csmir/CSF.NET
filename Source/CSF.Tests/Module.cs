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
        public IResult Test([Remainder] string help)
        {
            return Success(help);
        }

        [Command("test")]
        public IResult Test(string test = "", string test2 = "", int test3 = 3)
        {
            return Success("Success");
        }

        [Command("test")]
        [ErrorOverload]
        public IResult Test()
        {
            return Success("Success");
        }

        [Group("group")]
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
