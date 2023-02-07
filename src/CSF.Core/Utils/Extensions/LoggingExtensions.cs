using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    internal static class LoggingExtensions
    {
        public static Log AsLog(this IResult result, LogLevel overrideSeverity)
        {
            var severity = result.IsSuccess ? LogLevel.Information : overrideSeverity;

            return new Log(severity, result.ErrorMessage, result.Exception);
        }

        public static Log AsLog(this IConditionalComponent component, LogLevel severity)
        {
            var severity = LogLevel.Information;
        }
    }
}
