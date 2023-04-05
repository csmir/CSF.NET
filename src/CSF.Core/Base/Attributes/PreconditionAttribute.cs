using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class PreconditionAttribute : Attribute, IPrecondition
    {
        /// <inheritdoc/>
        public abstract ValueTask<PreconditionResult> CheckAsync(IContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken);

        /// <summary>
        ///     Returns the precondition with an error.
        /// </summary>
        /// <param name="errorMessage">The error message that occurred.</param>
        /// <param name="exception">The exception that occurred if available.</param>
        /// <returns>A failed precondition result.</returns>
        public PreconditionResult Error(string errorMessage, Exception exception = null)
            => PreconditionResult.Error(errorMessage, exception);

        /// <summary>
        ///     Returns the precondition with success.
        /// </summary>
        /// <returns>A successful precondition result.</returns>
        public PreconditionResult Success()
            => PreconditionResult.Success();
    }
}
