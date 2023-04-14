namespace CSF
{
    /// <summary>
    ///     Represents a registration and execution tool for modules.
    /// </summary>
    /// <typeparam name="T">The <see cref="ICommandContext"/> expected to use for this module.</typeparam>
    public abstract class ModuleBase<T> : ModuleBase
        where T : ICommandContext
    {
        private T _context;

        /// <summary>
        ///     Gets the command execution context containing value about the currently executed command.
        /// </summary>
        public new T Context
        {
            get
                => _context ??= (T)base.Context;
        }
    }

    /// <summary>
    ///     Represents a registration and execution tool for modules.
    /// </summary>
    public abstract class ModuleBase
    {
        /// <summary>
        ///     Gets the command execution context containing value about the currently executed command.
        /// </summary>
        public ICommandContext Context { get; internal set; }

        /// <summary>
        ///     Gets the component that displays all information about the command thats currently in scope.
        /// </summary>
        public Command Command { get; internal set; }

        /// <summary>
        ///     Executed before a command is ran.
        /// </summary>
        /// <param name="cancellationToken">Used to signal the command execution to be halted.</param>
        public virtual ValueTask BeforeExecuteAsync(CancellationToken cancellationToken)
            => ValueTask.CompletedTask;

        /// <summary>
        ///     Executed after a command is ran.
        /// </summary>
        /// <param name="cancellationToken">Used to signal the command execution to be halted.</param>
        public virtual ValueTask AfterExecuteAsync(CancellationToken cancellationToken)
            => ValueTask.CompletedTask;

        /// <summary>
        ///     Responds to the command with a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual void Respond(string message)
            => Console.WriteLine(message);
    }
}
