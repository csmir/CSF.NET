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
        public Action<object> SendAction { get; }

        /// <summary>
        ///     Creates a new default logging resolver with the provided sending action.
        /// </summary>
        /// <param name="sendAction"></param>
        public LogResolver(Action<object> sendAction)
        {
            _lock = new object();
            SendAction = sendAction;
        }

        /// <summary>
        ///     Uses the <see cref="SendAction"/> to send the provided message to the target handler.
        /// </summary>
        /// <param name="message"></param>
        public virtual void Send(object message)
        {
            lock (_lock)
                SendAction(message);
        }

        internal static LogResolver CreateLocalProvider()
            => new LogResolver((x) => Console.WriteLine(x));
    }
}
