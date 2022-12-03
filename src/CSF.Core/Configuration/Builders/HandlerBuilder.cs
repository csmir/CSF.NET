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
        public Func<IContext, IResult, CancellationToken, ValueTask> CommandResultDelegate { get; set; }

        /// <inheritdoc/>
        public Func<IConditionalComponent, CancellationToken, ValueTask> CommandRegistrationDelegate { get; set; }

        /// <inheritdoc/>
        public Func<IResultHandler, CancellationToken, ValueTask> HandlerRegistrationDelegate { get; set; }

        /// <inheritdoc/>
        public Func<ITypeReader, CancellationToken, ValueTask> ReaderRegistrationDelegate { get; set; }

        internal bool IsUselessToBuild
        {
            get
            {
                if (CommandResultDelegate is null
                    && CommandRegistrationDelegate is null
                    && HandlerRegistrationDelegate is null
                    && ReaderRegistrationDelegate is null)
                    return true;
                return false;
            }
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<IContext, IResult, CancellationToken, ValueTask> cmdResultHandle)
        {
            CommandResultDelegate = cmdResultHandle;
            return this;
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<IConditionalComponent, CancellationToken, ValueTask> cmdRegisterHandle)
        {
            CommandRegistrationDelegate = cmdRegisterHandle;
            return this;
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<IResultHandler, CancellationToken, ValueTask> rhrRegisterHandle)
        {
            HandlerRegistrationDelegate = rhrRegisterHandle;
            return this;
        }

        /// <inheritdoc/>
        public IHandlerBuilder ConfigureDelegate(Func<ITypeReader, CancellationToken, ValueTask> tprRegisterHandle)
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
