using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RemainderAttribute : Attribute
    {
    }
}
