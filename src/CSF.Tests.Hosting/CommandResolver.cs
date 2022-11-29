using CSF.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF.Tests.Hosting
{
    internal class CommandResolver : HostedCommandResolver<ICommandFramework, CommandContext>
    {
        public CommandResolver(ICommandFramework framework, IServiceProvider collection, ILogger<ICommandFramework> logger)
            : base(framework, collection, logger)
        {

        }

        protected override Task<CommandContext> GenerateContextAsync(string input, CancellationToken cancellationToken = default)
        {
            var ctx = new CommandContext(input);

            return Task.FromResult(ctx);
        }

        protected override Task<string> GetInputStreamAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                return Task.FromResult(Console.ReadLine()!);
            }
        }
    }
}
