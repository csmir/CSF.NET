namespace CSF.Core
{
    /// <summary>
    ///     Represents a container that implements an asynchronous functor to handle post-execution operations.
    /// </summary>
    /// <param name="handler">A functor that serves as the handler for post-execution operations in this resolver.</param>
    public class ResultResolver(Func<ICommandContext, ICommandResult, IServiceProvider, Task> handler)
    {
        private static readonly Lazy<ResultResolver> _i = new(() => new ResultResolver(null));

        /// <summary>
        ///     Gets the handler responsible for post-execution operation handling.
        /// </summary>
        public Func<ICommandContext, ICommandResult, IServiceProvider, Task> Handler { get; } = handler;

        internal Task TryHandleAsync(ICommandContext context, ICommandResult result, IServiceProvider services)
        {
            if (Handler == null)
            {
                return Task.CompletedTask;
            }

            return Handler(context, result, services);
        }

        internal static ResultResolver Default
        {
            get
            {
                return _i.Value;
            }
        }
    }
}
