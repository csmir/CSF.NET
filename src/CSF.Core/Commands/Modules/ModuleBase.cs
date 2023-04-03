using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    public abstract class ModuleBase
    {
        /// <summary>
        ///     Gets the command's context.
        /// </summary>
        public IContext Context { get; private set; }
        internal void SetContext(IContext context)
            => Context = context;

        /// <summary>
        ///     Displays all information about the command thats currently in scope.
        /// </summary>
        public Command CommandInfo { get; private set; }
        internal void SetInformation(Command info)
            => CommandInfo = info;

        /// <summary>
        ///     Formats and sends an error response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual ExecuteResult Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Formats and sends an error response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task<ExecuteResult> ErrorAsync(string message)
            => Task.FromResult(Error(message));

        /// <summary>
        ///     Formats and sends a successful response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual ExecuteResult Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();

            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Formats and sends a successful response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task<ExecuteResult> SuccessAsync(string message)
            => Task.FromResult(Success(message));

        /// <summary>
        ///     Formats and sends an informational response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual ExecuteResult Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Formats and sends an informational response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task<ExecuteResult> InfoAsync(string message)
            => Task.FromResult(Info(message));

        /// <summary>
        ///     Formats and sends a plain response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public virtual ExecuteResult Respond(string message)
        {
            Console.WriteLine(message);

            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Formats and sends a plain response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task<ExecuteResult> RespondAsync(string message)
            => Task.FromResult(Respond(message));

        /// <summary>
        ///     Invoked right before a command is executed.
        /// </summary>
        /// <remarks>
        ///     This method executes after all pipeline steps have resolved, it will always enter the command directly after.
        /// </remarks>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual Task BeforeExecuteAsync(Command info, CancellationToken cancellationToken)
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
        public virtual Task AfterExecuteAsync(Command info, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
