namespace CSF
{
    public readonly struct MatchResult : IResult
    {
        public Exception Exception { get; } = null;

        public Command Command { get; }

        public object[] Reads { get; }

        public bool Success { get; }

        internal MatchResult(Command command, object[] reads)
        {
            Command = command;
            Reads = reads;
            Success = true;
        }

        internal MatchResult(Command command, Exception exception)
        {
            Command = command;
            Reads = null;
            Success = false;

            Exception = exception;
        }
    }
}
