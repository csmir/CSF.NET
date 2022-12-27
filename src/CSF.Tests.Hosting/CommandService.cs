using CSF.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF.Tests.Hosting
{
    internal class CommandService : HostedCommandService<CommandFramework<CommandConveyor>, CommandContext>
    {
        public CommandService(CommandFramework<CommandConveyor> framework, IServiceProvider collection, ILogger<CommandFramework<CommandConveyor>> logger)
            : base(framework, collection, logger)
        {

        }
    }
}
