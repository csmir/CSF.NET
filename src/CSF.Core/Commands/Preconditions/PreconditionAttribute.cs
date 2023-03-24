using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    public abstract class PreconditionAttribute : Attribute, IPrecondition
    {
        /// <inheritdoc/>
        public abstract ValueTask<PreconditionResult> CheckAsync(IContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken);
    }
}
