using CSF.Core;
using Microsoft.Extensions.Logging;

namespace CSF.Hosting
{
#pragma warning disable CA2254 // Template should be a static expression
    public class HostedCommandContext(ILogger logger) : CommandContext
    {
        public ILogger Logger { get; } = logger;

        public override void LogTrace(string message, params object[] args)
        {
            Logger.Log(logLevel: LogLevel.Trace, message: message, args: args);
        }

        public override void LogDebug(string message, params object[] args)
        {
            Logger.Log(LogLevel.Debug, message, args);
        }

        public override void LogInformation(string message, params object[] args)
        {
            Logger.Log(LogLevel.Information, message, args);
        }

        public override void LogWarning(string message, params object[] args)
        {
            Logger.Log(LogLevel.Warning, message, args);
        }

        public override void LogError(string message, params object[] args)
        {
            Logger.Log(LogLevel.Error, message, args);
        }

        public override void LogCritical(string message, params object[] args)
        {
            Logger.Log(LogLevel.Critical, message, args);
        }
    }
#pragma warning restore CA2254 // Template should be a static expression
}
