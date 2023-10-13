namespace CSF.Samples.Hosting.Modules
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
            => Respond("Helped");
    }
}
