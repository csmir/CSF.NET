namespace CSF.Tests.Hosting
{
    public sealed class Module : ModuleBase<CommandContext>
    {
        private readonly IServiceProvider _provider;

        public Module(IServiceProvider provider)
        {
            _provider = provider;
        }

        [Command("help")]
        public void Help()
            => Success("Helped");
    }
}
