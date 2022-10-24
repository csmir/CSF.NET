using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     An attribute that can be used in implemented <see cref="ITypeReader"/>'s to default the value to a context property if its null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SelfIfNullAttribute : Attribute
    {

    }
}
