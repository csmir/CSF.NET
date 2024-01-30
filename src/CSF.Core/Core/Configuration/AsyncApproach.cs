using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core
{
    public enum AsyncApproach
    {
        Default = Await,

        Await = 0,

        Discard = 1,
    }
}
