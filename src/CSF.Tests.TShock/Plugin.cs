using CSF.TShock;
using Terraria;
using TerrariaApi.Server;

namespace CSF.Tests.TShock
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        private readonly TSCommandFramework _framework;

        public Plugin(Main game)
            : base(game)
        {
            _framework = new(new());
        }

        public override async void Initialize()
        {
            await _framework.BuildModulesAsync(typeof(Plugin).Assembly);
        }
    }
}