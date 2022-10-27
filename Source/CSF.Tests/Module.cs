namespace CSF.Tests
{
    internal class Module : ModuleBase<CommandContext>
    {
        [Command("help")]
        public IResult Help()
        {
            Logger.WriteDebug("Test message");

            return Success("Success");
        }

        [Command("loglevel")]
        public IResult LogLevel(LogLevel level)
        {
            Logger.LogLevel = level;

            return Success("Success");
        }
    }
}
