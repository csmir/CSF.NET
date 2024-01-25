namespace CSF.Exceptions
{
    internal sealed class ArgumentMissingException(string paramName, string message)
        : ArgumentException(message, paramName)
    {

    }
}
