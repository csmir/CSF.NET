namespace CSF
{
    /// <summary>
    ///     Defines a precondition attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public abstract class PreconditionAttribute : Attribute, IPrecondition
    {
        /// <inheritdoc/>
        public abstract void Check(IContext context, Command command, IServiceProvider provider);

        protected virtual void Fail(string message = null, Exception exception = null)
            => throw new CheckException(message, exception);
    }
}
