using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a provider for all <see cref="IResultHandler"/>'s in the program.
    /// </summary>
    public sealed class ResultHandlerProvider
    {
        private readonly List<IResultHandler> _handlers;

        /// <summary>
        ///     Creates a new <see cref="ResultHandlerProvider"/>.
        /// </summary>
        public ResultHandlerProvider()
            : this(new List<IResultHandler>())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="ResultHandlerProvider"/> from the provided list of result handlers.
        /// </summary>
        /// <param name="handlers">The default list of handlers to add.</param>
        public ResultHandlerProvider(List<IResultHandler> handlers)
        {
            _handlers = handlers;
        }

        /// <summary>
        ///     Adds a new <see cref="IResultHandler"/> to the provider.
        /// </summary>
        /// <param name="handler">The result handler to add.</param>
        /// <returns>The same <see cref="ResultHandlerProvider"/> for chaining calls.</returns>
        public ResultHandlerProvider Include(IResultHandler handler)
        {
            _handlers.Add(handler);
            return this;
        }

        /// <summary>
        ///     Removes an existing <see cref="IResultHandler"/> to the provider.
        /// </summary>
        /// <param name="handler">The result handler to remove.</param>
        /// <returns>The same <see cref="ResultHandlerProvider"/> for chainign calls.</returns>
        public ResultHandlerProvider Exclude(IResultHandler handler)
        {
            _handlers.Remove(handler);
            return this;
        }

        internal async Task InvokeCommandResultAsync(IContext context, IResult result, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
                await handler.OnCommandExecutedAsync(context, result, cancellationToken);
        }

        internal async Task InvokeBuildResultAsync(IConditionalComponent component, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
                await handler.OnCommandRegisteredAsync(component, cancellationToken);
        }

        internal async Task InvokeResultHandlerBuildResultAsync(IResultHandler resultHandler, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
                await handler.OnResultHandlerRegisteredAsync(resultHandler, cancellationToken);
        }

        internal async Task InvokeTypeReaderBuildResultAsync(ITypeReader typeReader, CancellationToken cancellationToken)
        {
            foreach (var handler in _handlers)
                await handler.OnTypeReaderRegisteredAsync(typeReader, cancellationToken);
        }
    }
}
