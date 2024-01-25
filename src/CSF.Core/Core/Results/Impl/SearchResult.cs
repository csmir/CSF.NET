using CSF.Reflection;

namespace CSF
{
    public readonly struct SearchResult : IResult
    {
        public Exception Exception { get; } = null;

        public CommandInfo Command { get; }

        public bool Success { get; }

        internal int SearchHeight { get; }

        internal SearchResult(CommandInfo command, int srcHeight)
        {
            Command = command;
            SearchHeight = srcHeight;
            Success = true;
        }

        internal SearchResult(Exception exception)
        {
            Exception = exception;
            Command = null;
            SearchHeight = 0;
            Success = false;
        }
    }
}
