using System;

namespace CSF
{
    /// <summary>
    ///     Specifies a parameter or property of the specified command module to not be injected with dependencies.
    /// </summary>
    /// <remarks>
    ///     This attribute is intended to go on the <see cref="InjectionConstructorAttribute"/>'s ctor parameters if set. 
    ///     If you only have one constructor defined, it goes on its parameters instead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DontInjectAttribute : Attribute
    {

    }
}
