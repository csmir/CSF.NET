using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    public interface IHandlerBuilder
    {
        /// <summary>
        ///     Gets or sets the handler delegate responsible for handling command results.
        /// </summary>
        Func<IContext, IResult, CancellationToken, ValueTask> CommandResultDelegate { get; set; }

        /// <summary>
        ///     Gets or sets the handler delegate responsible for handling <see cref="IConditionalComponent"/> registration results.
        /// </summary>
        Func<IConditionalComponent, CancellationToken, ValueTask> CommandRegistrationDelegate { get; set; }

        /// <summary>
        ///     Gets or sets the handler delegate responsible for handling <see cref="IResultHandler"/> registration results.
        /// </summary>
        Func<IResultHandler, CancellationToken, ValueTask> HandlerRegistrationDelegate { get; set; }

        /// <summary>
        ///     Gets or sets the handler delegate responsible for handling <see cref="ITypeReader"/> registration results.
        /// </summary>
        Func<ITypeReader, CancellationToken, ValueTask> ReaderRegistrationDelegate { get; set; }

        /// <summary>
        ///     Sets the handler responsible for handling command results.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>The same <see cref="IHandlerBuilder"/> for chaining calls.</returns>
        IHandlerBuilder ConfigureDelegate(Func<IContext, IResult, CancellationToken, ValueTask> cmdResultHandle);

        /// <summary>
        ///     Sets the handler responsible for handling <see cref="IConditionalComponent"/> registration results.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>The same <see cref="IHandlerBuilder"/> for chaining calls.</returns>
        IHandlerBuilder ConfigureDelegate(Func<IConditionalComponent, CancellationToken, ValueTask> cmdRegisterHandle);

        /// <summary>
        ///     Sets the handler responsible for handling <see cref="IResultHandler"/> registration results.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>The same <see cref="IHandlerBuilder"/> for chaining calls.</returns>
        IHandlerBuilder ConfigureDelegate(Func<IResultHandler, CancellationToken, ValueTask> rhrRegisterHandle);

        /// <summary>
        ///     Sets the handler responsible for handling <see cref="ITypeReader"/> registration results.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>The same <see cref="IHandlerBuilder"/> for chaining calls.</returns>
        IHandlerBuilder ConfigureDelegate(Func<ITypeReader, CancellationToken, ValueTask> tprRegisterHandle);

        /// <summary>
        ///     Builds the current <see cref="IHandlerBuilder"/> into a new <see cref="IResultHandler"/>.
        /// </summary>
        /// <returns>The newly created <see cref="IResultHandler"/>.</returns>
        IResultHandler Build();
    }
}
