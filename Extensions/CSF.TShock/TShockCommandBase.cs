using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using TShockAPI;
using System.Threading.Tasks;

namespace CSF.TShock
{
    public class TShockCommandBase : CommandBase<TShockCommandContext>
    {
        public override void RespondError(string message)
        {
            Context.Player.SendErrorMessage(message);
        }

        public override Task RespondErrorAsync(string message)
        {
            RespondError(message);
            return Task.CompletedTask;
        }

        public override void RespondInformation(string message)
        {
            Context.Player.SendInfoMessage(message);
        }

        public override Task RespondInformationAsync(string message)
        {
            RespondInformation(message);
            return Task.CompletedTask;
        }

        public override void RespondSuccess(string message)
        {
            Context.Player.SendSuccessMessage(message);
        }

        public override Task RespondSuccessAsync(string message)
        {
            RespondSuccess(message);
            return Task.CompletedTask;
        }

        public override void Respond(string message)
        {
            Respond(message, Color.LightGray);
        }

        public override Task RespondAsync(string message)
        {
            Respond(message, Color.LightGray);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Responds with the provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        public void Respond(string message, Color color)
        {
            Context.Player.SendMessage(message, color.R, color.G, color.B);
        }

        /// <summary>
        ///     Responds with the provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task RespondAsync(string message, Color color)
        {
            Respond(message, color);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Announce a message to the entire server with provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        public void Announce(string message, Color color)
        {
            TSPlayer.Server.SendMessage(message, color.R, color.G, color.B);
        }

        /// <summary>
        ///     Announce a message to the entire server with provided color.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color to send this message in.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task AnnounceAsync(string message, Color color)
        {
            Announce(message, color);
            return Task.CompletedTask;
        }
    }
}
