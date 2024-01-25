namespace CSF
{
    internal sealed class ArgumentMissingException(string paramName, string message)
        : ArgumentException(message, paramName)
    {

    }
}
