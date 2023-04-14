namespace CSF
{
    /// <summary>
    ///     Represents a generic <see cref="IModuleBase"/> to implement commands with.
    /// </summary>
    /// <typeparam name="T">The <see cref="IContext"/> expected to use for this command.</typeparam>
    public abstract class ModuleBase<T> : ModuleBase
        where T : IContext
    {
        private T _context;

        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public new T Context
        {
            get
                => _context ??= (T)base.Context;
        }
    }

    /// <summary>
    ///     Represents a <see cref="IModuleBase"/> to implement commands with.
    /// </summary>
    public abstract class ModuleBase : IModuleBase
    {
        /// <inheritdoc/>
        public IContext Context { get; internal set; }

        /// <inheritdoc/>
        public Command Command { get; internal set; }

        /// <inheritdoc/>
        public virtual ValueTask BeforeExecuteAsync(CancellationToken cancellationToken)
        {
            return new ValueTask(Task.CompletedTask);
        }

        /// <inheritdoc/>
        public virtual ValueTask AfterExecuteAsync(CancellationToken cancellationToken)
        {
            return new ValueTask(Task.CompletedTask);
        }

        /// <inheritdoc/>
        public virtual void Respond(string message)
        {
            Console.WriteLine(message);
        }
    }
}
