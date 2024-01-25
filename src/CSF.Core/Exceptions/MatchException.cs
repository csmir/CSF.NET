namespace CSF
{
    public class MatchException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {

    }
}
