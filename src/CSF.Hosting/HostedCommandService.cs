using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: CLSCompliant(true)]
namespace CSF.Hosting
{
    /// <summary>
    ///     Represents a hosted <see cref="CommandManager"/>. Provides the necessary extensions to set up a hosted environment for CSF.
    /// </summary>
    public abstract class HostedCommandService : CommandManager, IHostedService
    {
        protected HostedCommandService(IServiceProvider serviceProvider, ILogger<HostedCommandService> logger)
            : base(serviceProvider, logger)
        {
        }

        public abstract Task StartAsync(CancellationToken cancellationToken);

        public abstract Task StopAsync(CancellationToken cancellationToken);
    }
}
