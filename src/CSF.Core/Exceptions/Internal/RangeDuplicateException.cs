namespace CSF
{
    internal sealed class RangeDuplicateException(string paramName, string message)
        : ArgumentException(paramName, message)
    {

    }
}