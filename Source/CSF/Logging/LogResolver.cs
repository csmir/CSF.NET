using System;

namespace CSF
{
    /// <summary>
    ///     Represents the log resolver used to send messages to the target handler.
    /// </summary>
    public class LogResolver
    {
        private readonly object _lock;

        /// <summary>
        ///     The action used to log the provided response.
        /// </summary>
        public Action<ILog> SendAction { get; }

        /// <summary>
        ///     Creates a new default logging resolver with the provided sending action.
        /// </summary>
        /// <param name="sendAction"></param>
        public LogResolver(Action<ILog> sendAction)
        {
            _lock = new object();
            SendAction = sendAction;
        }

        /// <summary>
        ///     Uses the <see cref="SendAction"/> to send the provided message to the target handler.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(ILog message)
        {
            lock (_lock)
                SendAction(message);
        }

        internal static LogResolver CreateLocalProvider()
            => new LogResolver((x) =>
            {
                var message = $"{string.Format("{0:HH:mm:ss}", DateTime.UtcNow)} [{x.LogLevel}]: {x.Value}";

                if (x.Exception != null)
                    message += $"\n -> {x.Exception.ToString().Replace("\n\r", "\n\r -> ")}";

                Console.WriteLine(message);
            });
    }
}
