using CSF.Core;
using Microsoft.Extensions.Logging;

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
