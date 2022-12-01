using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <inheritdoc/>
    public abstract class ResultHandler : IResultHandler
    {
        /// <inheritdoc/>
        public virtual Task OnCommandRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task OnCommandExecutedAsync(IContext context, IResult result, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task OnTypeReaderRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task OnResultHandlerRegisteredAsync(IResultHandler resultHandler, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
