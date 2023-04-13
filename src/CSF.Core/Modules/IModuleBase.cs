namespace CSF
{
    /// <summary>
    ///     Represents a registration tool for modules.
    /// </summary>
    /// <remarks>
    ///     If you want to declare your own modules, define them by inheriting <see cref="ModuleBase"/> or <see cref="ModuleBase{T}"/>.
    /// </remarks>
    public interface IModuleBase
    {
        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public IContext Context { get; }

        /// <summary>
        ///     Displays all information about the command thats currently in scope.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        ///     Executed before a command is ran.
        /// </summary>
        /// <param name="cancellationToken">Used to signal the command execution to be halted.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/>.</returns>
        public ValueTask BeforeExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Executed after a command is ran.
        /// </summary>
        /// <param name="cancellationToken">Used to signal the command execution to be halted.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/>.</returns>
        public ValueTask AfterExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Responds to the command with a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A successful execution result.</returns>
        public void Respond(string message);
    }
}
