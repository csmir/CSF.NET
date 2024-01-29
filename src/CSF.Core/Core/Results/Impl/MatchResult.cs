using CSF.Reflection;

namespace CSF
{
    public readonly struct MatchResult : ICommandResult
    {
        public Exception Exception { get; } = null;

        public CommandInfo Command { get; }

        public object[] Reads { get; }

        public bool Success { get; }

        internal MatchResult(CommandInfo command, object[] reads)
        {
            Command = command;
            Reads = reads;
            Success = true;
        }

        internal MatchResult(CommandInfo command, Exception exception)
        {
            Command = command;
            Reads = null;
            Success = false;

            Exception = exception;
        }
    }
}
