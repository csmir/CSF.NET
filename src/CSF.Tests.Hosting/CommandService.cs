using CSF.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF.Tests.Hosting
{
    internal class CommandService : HostedCommandService<CommandFramework<PipelineService>, CommandContext>
    {
        public CommandService(CommandFramework<PipelineService> framework, IServiceProvider collection, ILogger<CommandFramework<PipelineService>> logger)
            : base(framework, collection, logger)
        {

        }
    }
}
