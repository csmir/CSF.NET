using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public class HostedModuleBase<T> : ModuleBase<T>
        where T : HostedCommandContext
    {
        public ILogger Logger
        {
            get
                => Context.Logger;
        }
    }
}
