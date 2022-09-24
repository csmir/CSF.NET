using CSF.Info;
using System;
using System.Threading.Tasks;

namespace CSF.Commands
{
    /// <summary>
    ///     Represents a generic commandbase to implement commands with.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CommandBase<T> : ICommandBase where T : ICommandContext
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
        public CommandStandardizationFramework Service { get; private set; }
        internal void SetService(CommandStandardizationFramework service)
            => Service = service;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task BeforeExecuteAsync(CommandInfo info, T context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task AfterExecuteAsync(CommandInfo info, T context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task ErrorAsync(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract Task SuccessAsync(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Information(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task InformationAsync();
    }
}
