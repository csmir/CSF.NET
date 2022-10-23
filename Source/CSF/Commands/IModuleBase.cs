using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents an interface used by <see cref="ModuleBase{T}"/> without needing to internally provide the generic parameter.
    /// </summary>
    /// <remarks>
    ///     Do not use this interface to build modules or new commandbases on. Use <see cref="ModuleBase{T}"/> instead.
    /// </remarks>
    internal interface ICommandBase
    {
        /// <summary>
        ///     Formats and sends an error response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void RespondError(string message);

        /// <summary>
        ///     Formats and sends an error response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task RespondErrorAsync(string message);

        /// <summary>
        ///     Formats and sends a successful response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void RespondSuccess(string message);

        /// <summary>
        ///     Formats and sends a successful response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task RespondSuccessAsync(string message);

        /// <summary>
        ///     Formats and sends an informational response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void RespondInformation(string message);

        /// <summary>
        ///     Formats and sends an informational response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task RespondInformationAsync(string message);

        /// <summary>
        ///     Formats and sends a plain response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Respond(string message);

        /// <summary>
        ///     Formats and sends a plain response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        Task RespondAsync(string message);
    }
}
