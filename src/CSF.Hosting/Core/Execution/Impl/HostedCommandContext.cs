using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public class HostedCommandContext : ICommandContext
    {
        internal Guid Id { get; }

        public ILogger Logger { get; }

        public HostedCommandContext(Guid id, ILogger logger)
        {
            Id = id;
            Logger = logger;
        }

        public void LogTrace(string message, params object[] args)
        {
            Logger.Log(LogLevel.Trace, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            Logger.Log(LogLevel.Debug, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            Logger.Log(LogLevel.Information, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            Logger.Log(LogLevel.Warning, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            Logger.Log(LogLevel.Error, message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            Logger.Log(LogLevel.Critical, message, args);
        }

        public override string ToString()
            => $"HostedCommandContext[{Id}]";
    }
}
