﻿namespace CSF.Core
{
    public sealed class ResultResolver(Func<ICommandContext, ICommandResult, IServiceProvider, Task> handler)
    {
        private static readonly Lazy<ResultResolver> _i = new(() => new ResultResolver(null));

        public Func<ICommandContext, ICommandResult, IServiceProvider, Task> Handler { get; } = handler;

        public Task TryHandleAsync(ICommandContext context, ICommandResult result, IServiceProvider services)
        {
            if (Handler == null)
            {
                return Task.CompletedTask;
            }

            return Handler(context, result, services);
        }

        public static ResultResolver Default
        {
            get
            {
                return _i.Value;
            }
        }
    }
}
