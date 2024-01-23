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
        ///     Gets the service provider used for executing the currently executed command.
        /// </summary>
        public IServiceProvider Services { get; internal set; }

        /// <summary>
        ///     Responds to the command with a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual void Respond(string message)
            => Console.WriteLine(message);
    }
}
