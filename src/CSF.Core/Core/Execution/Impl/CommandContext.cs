namespace CSF.Core
{
    public class CommandContext : ICommandContext
    {
        private readonly object _lock = new();
        private ICommandResult _fallback;

        public virtual void LogCritical(string message, params object[] args)
        {

        }

        public virtual void LogDebug(string message, params object[] args)
        {

        }

        public virtual void LogError(string message, params object[] args)
        {

        }

        public virtual void LogInformation(string message, params object[] args)
        {

        }

        public virtual void LogTrace(string message, params object[] args)
        {

        }

        public virtual void LogWarning(string message, params object[] args)
        {

        }

        bool ICommandContext.TryGetFallback(out ICommandResult result)
        {
            lock (_lock)
            {
                result = _fallback;

                return _fallback != null;
            }
        }

        void ICommandContext.TrySetFallback(ICommandResult result)
        {
            lock (_lock)
            {
                if (_fallback != null)
                {
                    _fallback = result;
                }
            }
        }
    }
}
