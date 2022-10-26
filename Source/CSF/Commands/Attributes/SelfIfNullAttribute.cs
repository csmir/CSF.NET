using System;

namespace CSF
{
    /// <summary>
    ///     An attribute that can be used in implemented <see cref="ITypeReader"/>'s to default the value to a context property if its null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class SelfIfNullAttribute : Attribute
    {

    }
}
