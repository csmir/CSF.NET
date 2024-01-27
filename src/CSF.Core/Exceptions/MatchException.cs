namespace CSF.Exceptions
{
    public class MatchException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {
        private const string _exHeader = "Command failed to reach execution state. View inner exception for more details.";
    }
}
