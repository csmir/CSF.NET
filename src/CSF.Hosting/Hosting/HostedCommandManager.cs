using CSF.Core;
using CSF.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

[assembly: CLSCompliant(true)]

namespace CSF.Hosting
{
    public class HostedCommandManager : CommandManager, IHostedService
    {
        public ILogger<HostedCommandManager> Logger { get; }

        public IActionFactory ActionFactory { get; }

        public HostedCommandManager(ILogger<HostedCommandManager> logger, IActionFactory factory, IServiceProvider services, CommandConfiguration configuration)
            : base(services, configuration)
        {
            ActionFactory = factory;
            Logger = logger;
        }

        public async Task ExecuteAsync(object[] args, CancellationToken cancellationToken)
        {
            var context = await ActionFactory.CreateContextAsync(cancellationToken).ConfigureAwait(false);

            await TryExecuteAsync(context, args, cancellationToken);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = RunAsync(cancellationToken);

            return Task.CompletedTask;
        }

        internal async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (cancellationToken.IsCancellationRequested)
                {
                    var args = await ActionFactory.CreateArgsAsync(cancellationToken).ConfigureAwait(false);

                    if (args == null)
                    {
                        ThrowHelpers.InvalidArg(args);
                    }

                    if (args.Length == 0)
                    {
                        ThrowHelpers.InvalidArg(args);
                    }

                    await ExecuteAsync(args, cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                // WIP
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}
