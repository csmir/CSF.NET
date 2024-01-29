using CSF.Core;
using CSF.Hosting;

namespace CSF.Tests.Hosting
{
    public sealed class Module : HostedModuleBase<HostedCommandContext>
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly IServiceProvider _provider;
#pragma warning restore IDE0052 // Remove unread private members

        public Module(IServiceProvider provider)
        {
            _provider = provider;
        }

        [Command("help")]
        public void Help()
            => Console.WriteLine("Helped");
    }
}
