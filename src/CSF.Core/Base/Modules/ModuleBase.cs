using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a generic command base to implement commands with.
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
        public Command Command { get; private set; }
        internal void SetCommand(Command info)
            => Command = info;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual ValueTask BeforeExecuteAsync(CancellationToken cancellationToken)
        {
            return new ValueTask(Task.CompletedTask);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual ValueTask AfterExecuteAsync(CancellationToken cancellationToken)
        {
            return new ValueTask(Task.CompletedTask);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual IResult Respond(string message)
        {
            Console.WriteLine(message);
            return ExecuteResult.FromSuccess();
        }
    }
}
