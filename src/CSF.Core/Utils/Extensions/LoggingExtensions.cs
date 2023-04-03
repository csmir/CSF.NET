using Microsoft.Extensions.Logging;

namespace CSF
{
    public static class LoggingExtensions
    {
        public static void LogResult(this ILogger logger, IResult result, LogLevel logLevel)
            => logger.Log(logLevel, result.Exception, "{}", result.ErrorMessage);
    }
}
