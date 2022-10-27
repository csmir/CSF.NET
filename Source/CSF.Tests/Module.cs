namespace CSF.Tests
{
    internal class Module : ModuleBase<CommandContext>
    {
        [Command("help")]
        public IResult Help()
        {
            Console.WriteLine("Success");

            Logger.WriteDebug("Test message");

            return ExecuteResult.FromSuccess();
        }

        [Command("loglevel")]
        public IResult LogLevel(LogLevel level)
        {
            Console.WriteLine("Success");

            Logger.LogLevel = level;

            return ExecuteResult.FromSuccess();
        }
    }
}
