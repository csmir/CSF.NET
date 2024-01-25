using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    internal sealed class ArgumentMissingException(string paramName, string message) 
        : ArgumentException(message, paramName)
    {

    }
}
