using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    internal sealed class RangeDuplicateException(string paramName, string message) 
        : ArgumentException(paramName, message)
    {

    }
}