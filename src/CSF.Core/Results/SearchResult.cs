namespace CSF
{
    public readonly struct SearchResult : IResult
    {
        public Exception Exception { get; } = null;

        public Command Command { get; }

        internal int SearchHeight { get; }

        internal SearchResult(Command command, int srcHeight)
        {
            Command = command;
            SearchHeight = srcHeight;
        }

        internal SearchResult(Exception exception)
        {
            Exception = exception;
            Command = null;
            SearchHeight = 0;
        }
    }
}
