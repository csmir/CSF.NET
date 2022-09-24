using CSF.Commands;
using CSF.Info;
using CSF.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Preconditions
{
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckAsync(ICommandContext context, CommandInfo info, IServiceProvider provider);
    }
}
