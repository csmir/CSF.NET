﻿using System.ComponentModel;

namespace CSF.Core
{
    /// <summary>
    ///     A container that implements an asynchronous functor to handle post-execution operations.
    /// </summary>
    /// <param name="handler">A functor that serves as the handler for post-execution operations in this resolver.</param>
    public class ResultResolver(Func<ICommandContext, ICommandResult, IServiceProvider, Task> handler)
    {
        private static readonly Lazy<ResultResolver> _i = new(() => new ResultResolver(null));

        /// <summary>
        ///     Gets the handler responsible for post-execution operation handling.
        /// </summary>
        public Func<ICommandContext, ICommandResult, IServiceProvider, Task> Handler { get; } = handler;

        /// <summary>
        ///     Validates the state of the <see cref="Handler"/> and attempts to execute the delegate.
        /// </summary>
        /// <param name="context">Context of the current execution.</param>
        /// <param name="result">The result of the command, being successful or containing failure information.</param>
        /// <param name="services">The provider used to register modules and inject services.</param>
        /// <returns>An awaitable <see cref="Task"/> that waits for the delegate to finish.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Task TryHandleAsync(ICommandContext context, ICommandResult result, IServiceProvider services)
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
