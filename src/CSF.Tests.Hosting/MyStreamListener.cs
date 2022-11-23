using CSF.Hosting;
using Microsoft.Extensions.Logging;

namespace CSF.Tests.Hosting
{
    internal class MyStreamListener : CommandStreamListener<CommandFramework, CommandContext>
    {
        public MyStreamListener(CommandFramework framework, IServiceProvider collection, ILogger<CommandFramework> logger)
            : base(framework, collection, logger)
        {

        }

        protected override Task<CommandContext> GenerateContextAsync(string input, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task<string> GetInputStreamAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
