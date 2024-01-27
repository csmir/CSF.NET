using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public class HostedContextFactory : IContextFactory<HostedCommandContext>
    {
        public ILoggerFactory LoggerFactory { get; }

        public HostedContextFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        public virtual HostedCommandContext CreateContext()
        {
            var guid = Guid.NewGuid();
            var logger = LoggerFactory.CreateLogger($"CSF.Command.Pipeline[{Guid.NewGuid()}]");

            logger.LogTrace("Generating context with ID {}", guid);

            return new(guid, logger);
        }

        public void Dispose()
        {
            
        }
    }
}
