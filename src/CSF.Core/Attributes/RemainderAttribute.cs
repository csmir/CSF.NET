namespace CSF
{
    /// <summary>
    ///     Defines that this parameter should be the remainder of the command phrase.
    /// </summary>
    /// <remarks>
    ///     This attribute can only be set on the last parameter of a command method.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RemainderAttribute : Attribute
    {

    }
}
