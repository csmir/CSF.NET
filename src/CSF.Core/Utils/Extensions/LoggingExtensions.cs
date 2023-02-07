namespace CSF
{
    internal static class LoggingExtensions
    {
        public static Log AsLog(this IResult result, LogLevel? overrideSeverity = null)
        {
            var severity = overrideSeverity ?? (result.IsSuccess ? LogLevel.Information : LogLevel.Error);

            return new Log(severity, result.ErrorMessage, result.Exception);
        }
    }
}
