using CSF.TShock;
using Terraria;
using TerrariaApi.Server;

namespace CSF.Samples.TShock5
{
    [ApiVersion(2, 1)]
    public sealed class Plugin : TerrariaPlugin
    {
        private readonly TSCommandFramework _fx;

        public Plugin(Main game)
            : base(game)
        {
            // Define the command standardization framework made for TShock.
            _fx = new(new CommandConfiguration()
            {
                DoAsynchronousExecution = false
            });
        }

        public override async void Initialize()
        {
            // Build the modules available in the current assembly.
            await _fx.BuildModulesAsync(typeof(Plugin).Assembly);
        }
    }
}