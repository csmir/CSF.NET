using CSF.Reflection;

namespace CSF
{
    public readonly struct RunResult : IResult
    {
        public Exception Exception { get; } = null;

        public object ReturnType { get; } = null;

        public CommandInfo Command { get; }

        public bool Success { get; }

        internal RunResult(CommandInfo command, Exception exception)
        {
            Exception = exception;
            Command = command;
            Success = false;
        }

        internal RunResult(CommandInfo command, object returnValue)
        {
            ReturnType = returnValue;
            Command = command;
            Success = true;
        }
    }
}
