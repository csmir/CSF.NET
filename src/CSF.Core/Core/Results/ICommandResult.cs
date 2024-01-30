namespace CSF
{
    public interface ICommandResult
    {
        public Exception Exception { get; }

        public bool Success { get; }
    }
}
