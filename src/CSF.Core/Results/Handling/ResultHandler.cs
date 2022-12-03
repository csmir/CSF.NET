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
        public virtual ValueTask OnCommandRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        /// <inheritdoc/>
        public virtual ValueTask OnCommandExecutedAsync(IContext context, IResult result, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        /// <inheritdoc/>
        public virtual ValueTask OnTypeReaderRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }

        /// <inheritdoc/>
        public virtual ValueTask OnResultHandlerRegisteredAsync(IResultHandler resultHandler, CancellationToken cancellationToken)
        {
            return new ValueTask();
        }
    }
}
