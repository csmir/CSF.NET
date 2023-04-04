using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: CLSCompliant(true)]
namespace CSF.Hosting
{
    /// <summary>
    ///     Represents a hosted <see cref="CommandFramework"/>. Provides the necessary extensions to set up a hosted environment for CSF.
    /// </summary>
    public abstract class HostedCommandService : CommandFramework, IHostedService
    {
        public HostedCommandService(ComponentContainer components, FrameworkBuilderContext context, IServiceProvider serviceProvider, ILogger<CommandFramework> logger, ICommandConveyor conveyor) 
            : base(components, context, serviceProvider, logger, conveyor)
        {

        }

        public abstract Task StartAsync(CancellationToken cancellationToken);

        public abstract Task StopAsync(CancellationToken cancellationToken);
    }
}
