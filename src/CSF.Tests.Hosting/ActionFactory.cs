using CSF.Helpers;
using CSF.Hosting;
using CSF.Parsing;
using Microsoft.Extensions.Logging;

namespace CSF.Tests.Hosting
{
    public class ActionFactory : IActionFactory<HostedCommandContext>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly StringParser _stringParser;

        public ActionFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _stringParser = new StringParser();
        }

        public ValueTask<object[]> CreateArgsAsync(CancellationToken cancellationToken)
        {
            var input = Console.ReadLine();

            var args = _stringParser.Parse(input);

            if (args.Length == 0)
            {
                ThrowHelpers.InvalidArg(args);
            }

            return ValueTask.FromResult(args);
        }

        public ValueTask<HostedCommandContext> CreateContextAsync(CancellationToken cancellationToken)
        {
            var guid = Guid.NewGuid();
            var logger = _loggerFactory.CreateLogger($"CSF.Command.Pipeline[{Guid.NewGuid()}]");

            logger.LogTrace("Generating context with ID {}", guid);

            return ValueTask.FromResult(new HostedCommandContext(logger));
        }

        public void Dispose()
        {

        }
    }
}
