using System.Runtime.CompilerServices;

namespace CSF
{
    /// <summary>
    ///     Represents a result that was produced by a command pipeline.
    /// </summary>
    public readonly struct FailedResult : IResult
    {
        /// <summary>
        ///     Gets the code that represents where the pipeline failed.
        /// </summary>
        public FailureCode Code { get; } = FailureCode.Execute;

        /// <summary>
        ///     The exception thrown by the pipeline in case of failure. In most cases, this is a <see cref="PipelineException" />. The <see cref="Code"/> is associated with the following exceptions:
        /// </summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><see cref="FailureCode.Search"/> - <see cref="SearchException"/>. Thrown when no command could be found.</item>
        ///         <item><see cref="FailureCode.Check"/> - <see cref="CheckException"/>. Thrown when no matched command succeeded its precondition checks.</item>
        ///         <item><see cref="FailureCode.Read"/> - <see cref="ReadException"/>. Thrown when no matched command succeeded parsing its parameters.</item>
        ///         <item><see cref="FailureCode.Execute"/> - <see cref="ExecuteException"/>. Thrown when the command being executed failed to run its body.</item>
        ///     </list>
        ///     <i>This range is determined by the execution flow. Exceptions will occur in this order.</i>
        /// </remarks>
        public Exception Exception { get; } = null;

        /// <summary>
        ///     Creates a new <see cref="FailedResult"/> from the provided result code and exception.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="exception"></param>
        public FailedResult(FailureCode code, Exception exception = null)
        {
            Code = code;
            Exception = exception;
        }

        /// <inheritdoc />
        public bool Failed()
            => true;

        /// <inheritdoc />
        public TaskAwaiter GetAwaiter()
            => Task.CompletedTask.GetAwaiter();

        /// <summary>
        ///     Formats a readable output from this <see cref="FailedResult"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"[{Code}] " + Exception;
    }
}
