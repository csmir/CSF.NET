using CSF.Reflection;

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
        ///     Gets the command execution services as provided by the command scope.
        /// </summary>
        public IServiceProvider Services { get; internal set; }

        /// <summary>
        ///     Gets the component that displays all information about the command thats currently in scope.
        /// </summary>
        public CommandInfo Command { get; internal set; }

        /// <summary>
        ///     Responds to the command with a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual void Respond(string message)
            => Console.WriteLine(message);

        public virtual RunResult ReturnTypeResolve(object value)
        {
            switch (value)
            {
                case Task task:
                    return new(Command, task);
                case null:
                    return new(Command, null);
                default:
                    throw new NotSupportedException($"The return value of the command in question is not supported. Consider overriding {nameof(ReturnTypeResolve)} to add your own return type resolver.");
            }
        }

        public virtual ValueTask BeforeExecuteAsync()
        {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask AfterExecuteAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
