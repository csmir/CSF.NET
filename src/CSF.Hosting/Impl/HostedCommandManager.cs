using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

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
        ///     Enters a loop 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task RunAsync(CancellationToken cancellationToken)
        {
            var parser = new TextParser();

            while (true)
            {
                var input = Console.ReadLine()
                    ?? throw new InvalidOperationException();

                if (parser.TryParse(input, out var output))
                {
                    var context = new CommandContext(output);

                    var result = await TryExecuteAsync(context, cancellationToken: cancellationToken);

                    if (!result.IsSuccess)
                        Logger.LogError(result.Exception, "{}", result.ErrorMessage);
                }
            }
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
