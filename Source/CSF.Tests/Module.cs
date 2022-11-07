namespace CSF.Tests
{
    internal class Module : ModuleBase<CommandContext>
    {
        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        [Command("help")]
        public IResult Help()
        {
            Logger.WriteDebug("Test message");

            return Success("Success");
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [Command("loglevel")]
        public IResult LogLevel(LogLevel level)
        {
            Logger.LogLevel = level;

            return Success("Success");
        }
    }
}
