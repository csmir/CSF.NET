using CSF.Reflection;

namespace CSF
{
    public abstract class CommandContext : ICommandContext
    {
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
    }
}
