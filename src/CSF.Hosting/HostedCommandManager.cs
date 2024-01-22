using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF
{
    /// <summary>
    ///     Represents a host-managed command manager.
    /// </summary>
    public class HostedCommandManager : CommandManager, IHostedService
    {
        /// <summary>
        ///     The logger used by the hosted manager.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        ///     The parser used by the hosted manager.
        /// </summary>
        public TextParser Parser { get; }

        public HostedCommandManager(IServiceProvider serviceProvider)
            : this(serviceProvider.GetRequiredService<ILogger<HostedCommandManager>>(), serviceProvider)
        {

        }

        public HostedCommandManager(ILogger logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Logger = logger;
            Parser = new TextParser();
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () => await RunAsync(cancellationToken), cancellationToken)
                .ContinueWith(async _ => await StopAsync(cancellationToken));

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Enters a loop through which commands are read and ran.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var context = new CommandContext(Console.ReadLine(), Parser);

                var result = ExecuteAsync(context, new ExecutionOptions());

                if (result.Failed(out var failure))
                    Logger.LogError(failure.Exception, "Command execution returned an exception.");

                await result;
            }
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
