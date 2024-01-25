namespace CSF.Exceptions
{
    public class MatchException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {

    }
}
