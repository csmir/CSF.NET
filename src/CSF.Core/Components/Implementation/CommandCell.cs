namespace CSF
{
    public readonly struct CommandCell
    {
        public Command Command { get; }

        public object[] Arguments { get; }

        public Exception Exception { get; }

        public bool IsInvalid { get; }

        public CommandCell(Exception exception)
            : this(null, null)
        {
            Command = null;
            Arguments = null;

            Exception = exception;
            IsInvalid = true;
        }

        public CommandCell(Command match, object[] arguments)
        {
            Command = match;
            Arguments = arguments;

            Exception = null;
            IsInvalid = false;
        }
    }
}
