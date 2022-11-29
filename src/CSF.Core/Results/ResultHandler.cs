using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    /// <inheritdoc/>
    public abstract class ResultHandler : IResultHandler
    {
        /// <inheritdoc/>
        public virtual void OnResult(IContext context, IResult result, CommandInfo command = null)
        {
            return;
        }

        /// <inheritdoc/>
        public virtual Task OnResultAsync(IContext context, IResult result, CommandInfo command = null)
        {
            return Task.CompletedTask;
        }
    }
}
