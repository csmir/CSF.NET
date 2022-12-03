using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default layer of handling inbound command results.
    /// </summary>
    public interface IResultHandler
    {
        /// <summary>
        ///     Invoked when <see cref="CommandFramework{T}.ExecuteCommandAsync{TContext}(TContext, System.IServiceProvider, CancellationToken)"/> returns a result.
        /// </summary>
        /// <param name="context">The <see cref="IContext"/> used to invoke this command.</param>
        /// <param name="result">The result of this execution.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        ValueTask OnCommandExecutedAsync(IContext context, IResult result, CancellationToken cancellationToken);

        /// <summary>
        ///     Invoked when <see cref="CommandFramework{T}.BuildModuleAsync(System.Type)"/> returns a result.
        /// </summary>
        /// <param name="component">The registered component.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        ValueTask OnCommandRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken);

        /// <summary>
        ///     Invoked when <see cref="CommandFramework{T}.BuildTypeReaderAsync(System.Type)"/> returns a result.
        /// </summary>
        /// <param name="typeReader">The registered typereader.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        ValueTask OnTypeReaderRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken);

        /// <summary>
        ///     Invoked when <see cref="CommandFramework{T}.BuildResultHandlerAsync(System.Type)"/> returns a result.
        /// </summary>
        /// <param name="resultHandler">The registered result handler. This includes the current handler, which is the first time this method is called.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        ValueTask OnResultHandlerRegisteredAsync(IResultHandler resultHandler, CancellationToken cancellationToken);
    }
}
