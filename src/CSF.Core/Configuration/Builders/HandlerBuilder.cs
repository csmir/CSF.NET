using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    public sealed class HandlerBuilder : IHandlerBuilder
    {
        /// <inheritdoc/>
        public Func<IContext, IResult, CancellationToken, Task> CommandResultDelegate { get; set; }

        /// <inheritdoc/>
        public Func<IConditionalComponent, CancellationToken, Task> CommandRegistrationDelegate { get; set; }

        /// <inheritdoc/>
        public Func<IResultHandler, CancellationToken, Task> HandlerRegistrationDelegate { get; set; }

        /// <inheritdoc/>
        public Func<ITypeReader, CancellationToken, Task> ReaderRegistrationDelegate { get; set; }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<IContext, IResult, CancellationToken, Task> cmdResultHandle)
        {
            CommandResultDelegate = cmdResultHandle;
            return this;
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<IConditionalComponent, CancellationToken, Task> cmdRegisterHandle)
        {
            CommandRegistrationDelegate = cmdRegisterHandle;
            return this;
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<IResultHandler, CancellationToken, Task> rhrRegisterHandle)
        {
            HandlerRegistrationDelegate = rhrRegisterHandle;
            return this;
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<ITypeReader, CancellationToken, Task> tprRegisterHandle)
        {
            ReaderRegistrationDelegate = tprRegisterHandle;
            return this;
        }

        /// <inheritdoc/>
        public IResultHandler Build()
        {
            return new DefaultResultHandler(CommandResultDelegate, CommandRegistrationDelegate, HandlerRegistrationDelegate, ReaderRegistrationDelegate);
        }
    }
}
