using System.Runtime.InteropServices;

namespace CSF
{
    /// <summary>
    ///     Represents a command resolved through the match and search pipeline.
    /// </summary>
    public readonly struct CommandCell
    {
        /// <summary>
        ///     Gets the command that this cell represents.
        /// </summary>
        public Command Command { get; }

        /// <summary>
        ///     Gets the arguments that are to be used to execute the method of this command.
        /// </summary>
        public object[] Arguments { get; }

        /// <summary>
        ///     Gets the exception that occurred while resolving this command.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if no exception occurred.
        /// </remarks>
        public Exception Exception { get; }

        /// <summary>
        ///     Gets if the cell is invalid and cannot be executed.
        /// </summary>
        public bool IsInvalid { get; }

        /// <summary>
        ///     Gets the priority of the command, where higher values take priority over lower ones.
        /// </summary>
        public byte Priority { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandResult"/> that is constructed when resolvement of a command failed.
        /// </summary>
        /// <param name="exception"></param>
        public CommandCell(Exception exception)
            : this(null, null)
        {
            Command = null;
            Arguments = null;

            Exception = exception;
            IsInvalid = true;
        }

        /// <summary>
        ///     Creates a new <see cref="CommandResult"/> that is constructed when resolvement of a command succeeded.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="arguments"></param>
        public CommandCell(Command match, object[] arguments)
        {
            Command = match;
            Arguments = arguments;
            Priority = match.Priority;

            Exception = null;
            IsInvalid = false;
        }
    }
}
