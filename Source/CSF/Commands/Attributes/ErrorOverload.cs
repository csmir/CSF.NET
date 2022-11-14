using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Represents an command that is called when no best match was found.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ErrorOverloadAttribute : Attribute
    {

    }
}
