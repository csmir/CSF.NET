using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a generic command base to implement commands with.
    /// </summary>
    /// <typeparam name="T">The <see cref="ICommandContext"/> expected to use for this command.</typeparam>
    public abstract class ModuleBase<T> : ICommandBase where T : ICommandContext
    {
        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public T Context { get; private set; }
        internal void SetContext(T context)
            => Context = context;

        /// <summary>
        ///     Displays all information about the command thats currently in scope.
        /// </summary>
        public CommandInfo CommandInfo { get; private set; }
        internal void SetInformation(CommandInfo info)
            => CommandInfo = info;

        /// <summary>
        ///     The command service used to execute this command.
        /// </summary>
        public CommandFramework Framework { get; private set; }
        internal void SetSource(CommandFramework service)
            => Framework = service;

        /// <summary>
        ///     The logger of the framework being used.
        /// </summary>
        public ILogger Logger { get; private set; }
        internal void SetLogger(ILogger logger)
            => Logger = logger;

        /// <summary>
        ///     Invoked right before a command is executed.
        /// </summary>
        /// <remarks>
        ///     This method executes after all pipeline steps have resolved, it will always enter the command directly after.
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task BeforeExecuteAsync(CommandInfo info, T context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Invoked right after a command is executed.
        /// </summary>
        /// <remarks>
        ///     This method does not execute if the command fails to finish execution or returns an error.
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task AfterExecuteAsync(CommandInfo info, T context)
        {
            return Task.CompletedTask;
        }

        public virtual void RespondError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public virtual Task RespondErrorAsync(string message)
        {
            RespondError(message);
            return Task.CompletedTask;
        }

        public virtual void RespondSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public virtual Task RespondSuccessAsync(string message)
        {
            RespondSuccess(message);
            return Task.CompletedTask;
        }

        public virtual void RespondInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public virtual Task RespondInformationAsync(string message)
        {
            RespondInformation(message);
            return Task.CompletedTask;
        }

        public virtual void Respond(string message)
        {
            Console.WriteLine(message);
        }

        public virtual Task RespondAsync(string message)
        {
            Respond(message);
            return Task.CompletedTask;
        }
    }
}
