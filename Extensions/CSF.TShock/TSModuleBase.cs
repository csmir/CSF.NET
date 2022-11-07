using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSF.TShock
{
    /// <inheritdoc/>
    public class TSModuleBase<T> : ModuleBase<T>
        where T : ITSCommandContext
    {
        /// <inheritdoc/>
        public override ExecuteResult Error(string message, params object[] values)
        {
            Context.Player.SendErrorMessage(message, values);
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> ErrorAsync(string message, params object[] values)
        {
            return Task.FromResult(Error(message, values));
        }

        /// <summary>
        ///     Responds with a multi-match error.
        /// </summary>
        /// <param name="matches">The found matches.</param>
        public ExecuteResult Error(IEnumerable<object> matches)
        {
            Context.Player.SendMultipleMatchError(matches);
            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Responds with a multi-match error.
        /// </summary>
        /// <param name="matches">The found matches.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task<ExecuteResult> ErrorAsync(IEnumerable<object> matches)
        {
            return Task.FromResult(Error(matches));
        }

        /// <inheritdoc/>
        public override ExecuteResult Info(string message, params object[] values)
        {
            Context.Player.SendInfoMessage(message, values);
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> InfoAsync(string message, params object[] values)
        {
            return Task.FromResult(Info(message, values));
        }

        /// <inheritdoc/>
        public override ExecuteResult Success(string message, params object[] values)
        {
            Context.Player.SendSuccessMessage(message, values);
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> SuccessAsync(string message, params object[] values)
        {
            return Task.FromResult(Success(message, values));
        }

        /// <inheritdoc/>
        public override ExecuteResult Respond(string message, params object[] values)
        {
            return Respond(string.Format(message, values), Color.LightGray);
        }

        /// <inheritdoc/>
        public override Task<ExecuteResult> RespondAsync(string message, params object[] values)
        {
            return Task.FromResult(Respond(string.Format(message, values), Color.LightGray));
        }

        /// <summary>
        ///     Responds with the provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        public ExecuteResult Respond(string message, Color color)
        {
            Context.Player.SendMessage(message, color);
            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Responds with the provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task<ExecuteResult> RespondAsync(string message, Color color)
        {
            return Task.FromResult(Respond(message, color));
        }

        /// <summary>
        ///     Announce a message to the entire server with provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        public ExecuteResult Announce(string message, Color color)
        {
            TShockAPI.TShock.Utils.Broadcast(message, color);
            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Announce a message to the entire server with provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task<ExecuteResult> AnnounceAsync(string message, Color color)
        {
            return Task.FromResult(Announce(message, color));
        }
    }
}
