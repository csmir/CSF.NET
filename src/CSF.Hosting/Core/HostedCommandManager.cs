using CSF.Reflection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public class HostedCommandManager : CommandManager
    {
        public ILogger<HostedCommandManager> Logger { get; }

        public IContextFactory ContextFactory { get; }

        public HostedCommandManager(ILogger<HostedCommandManager> logger, IContextFactory factory, IServiceProvider services, CommandConfiguration configuration) 
            : base(services, configuration)
        {
            ContextFactory = factory;
            Logger = logger;
        }

        public Task<IResult> ExecuteAsync(params object[] args)
        {
            var context = ContextFactory.CreateContext();

            return base.ExecuteAsync(context, args);
        }
    }
}
