namespace CSF.Tests.Modules
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
            [Command("test")]
            public void Test()
                => Respond("");

            [Command("test")]
            public void Test1()
                => Respond("");

            [Command("test")]
            public void Test2()
                => Respond("");

            [Command("test")]
            public void Test3()
                => Respond("");

            [Group("group")]
            public class InnerInnerModule : ModuleBase<CommandContext>
            {
                [Command("test")]
                public void Test()
                    => Respond("");

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
