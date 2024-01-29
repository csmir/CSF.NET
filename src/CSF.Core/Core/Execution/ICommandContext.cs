using System.Diagnostics.CodeAnalysis;

namespace CSF.Core
{
    public interface ICommandContext
    {
        public void LogTrace(string message, params object[] args);

        public void LogDebug(string message, params object[] args);

        public void LogInformation(string message, params object[] args);

        public void LogWarning(string message, params object[] args);

        public void LogError(string message, params object[] args);

        public void LogCritical(string message, params object[] args);

        internal bool TryGetFallback([NotNullWhen(true)] out ICommandResult result);

        internal void TrySetFallback(ICommandResult result);
    }
}
