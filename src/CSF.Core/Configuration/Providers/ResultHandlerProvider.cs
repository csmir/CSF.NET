using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    public sealed class ResultHandlerProvider
    {
        private List<IResultHandler> _handlers;

        public ResultHandlerProvider()
            : this(new List<IResultHandler>())
        {

        }

        public ResultHandlerProvider(List<IResultHandler> handlers)
        {
            _handlers = handlers;
        }

        public ResultHandlerProvider Include(IResultHandler handler)
        {
            _handlers.Add(handler);
            return this;
        }

        public ResultHandlerProvider Exclude(IResultHandler handler)
        {
            _handlers.Remove(handler);
            return this;
        }

        public async Task InvokeHandlers(IContext context, IResult result, CommandInfo command = null)
        {
            foreach (var handler in _handlers)
            {
                await handler.OnResultAsync(context, result, command);
            }
        }
    }
}
