namespace CSF.Tests
{
    internal class Module : ModuleBase<CommandContext>
    {
        [Command("loglevel")]
        public IResult LogLevel(LogLevel level)
        {
            Logger.LogLevel = level;

            return Success("Success");
        }

        [Group("group")]
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
