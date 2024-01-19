namespace CSF
{
    public readonly struct RunResult : IResult
    {
        public Exception Exception { get; } = null;

        public object ReturnType { get; } = null;

        public Command Command { get; }

        public bool Success { get; }

        internal RunResult(Command command, Exception exception)
        {
            Exception = exception;
            Command = command;
            Success = false;
        }

        internal RunResult(Command command, object returnValue)
        {
            ReturnType = returnValue;
            Command = command;
            Success = true;
        }
    }
}
