namespace CSF
{
    /// <summary>
    ///     Represents the error code that occurred during command execution. <see cref="None"/> means there was no negative result.
    /// </summary>
    public enum ResultCode : int
    {
        /// <summary>
        ///     Represents no negative result.
        /// </summary>
        None,

        /// <summary>
        ///     Represents a result tied to <see cref="SearchException"/>. Thrown when no command could be found.
        /// </summary>
        Search,

        /// <summary>
        ///     Represents a result tied to <see cref="CheckException"/>. Thrown when no matched command succeeded its precondition checks.
        /// </summary>
        Check,

        /// <summary>
        ///     Represents a result tied to <see cref="ReadException"/>. Thrown when no matched command succeeded parsing its parameters.
        /// </summary>
        Read,

        /// <summary>
        ///     Represents a result tied to <see cref="ExecuteException"/>. Thrown when the command being executed failed to run its body.
        /// </summary>
        Execute,
    }

    /// <summary>
    ///     Represents a result that was produced by a command pipeline.
    /// </summary>
    public readonly struct CommandResult
    {
        /// <summary>
        ///     Gets the code that represents where the pipeline failed. <see cref="ResultCode.None"/> if none. 
        /// </summary>
        public ResultCode Code { get; } = ResultCode.None;

        /// <summary>
        ///     The exception thrown by the pipeline in case of failure. In most cases, this is a <see cref="PipelineException" />. The <see cref="Code"/> is associated with the following exceptions:
        /// </summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><see cref="ResultCode.Search"/> - <see cref="SearchException"/>. Thrown when no command could be found.</item>
        ///         <item><see cref="ResultCode.Check"/> - <see cref="CheckException"/>. Thrown when no matched command succeeded its precondition checks.</item>
        ///         <item><see cref="ResultCode.Read"/> - <see cref="ReadException"/>. Thrown when no matched command succeeded parsing its parameters.</item>
        ///         <item><see cref="ResultCode.Execute"/> - <see cref="ExecuteException"/>. Thrown when the command being executed failed to run its body.</item>
        ///     </list>
        ///     <i>This range is determined by the execution flow. Exceptions will occur in this order.</i>
        /// </remarks>
        public Exception Exception { get; } = null;

        /// <summary>
        ///     Creates a new <see cref="CommandResult"/> from the provided result code and exception.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="exception"></param>
        public CommandResult(ResultCode code, Exception exception = null)
        {
            Code = code;
            Exception = exception;
        }

        /// <summary>
        ///     Formats a readable output from the result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"[{Code}]" + Exception;
    }
}
