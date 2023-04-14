using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <inheritdoc/>
        public abstract void Check(IContext context, Command command, IServiceProvider provider);

        [DoesNotReturn]
        protected virtual void Fail(string message = null, Exception exception = null)
            => throw new CheckException(message, exception);
    }
}
