using CSF.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CSF.Tests
{
    public sealed class Module(IServiceProvider provider) : HostedModuleBase<HostedCommandContext>
    {
        private readonly IServiceProvider _provider = provider;

        [Command("stop")]
        public void Stop()
        {
            var handler = _provider.GetRequiredService<CommandHandler>();

            handler.StopAsync(default);
        }
    }
}
