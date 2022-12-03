using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    internal class DefaultResultHandler : ResultHandler
    {
        public Func<IContext, IResult, CancellationToken, ValueTask> CommandResultDelegate { get; }

        public Func<IConditionalComponent, CancellationToken, ValueTask> CommandRegistrationDelegate { get; }

        public Func<IResultHandler, CancellationToken, ValueTask> HandlerRegistrationDelegate { get; }

        public Func<ITypeReader, CancellationToken, ValueTask> ReaderRegistrationDelegate { get; }

        internal DefaultResultHandler(
            Func<IContext, IResult, CancellationToken, ValueTask> cmdfunc, 
            Func<IConditionalComponent, CancellationToken, ValueTask> cmdregfunc, 
            Func<IResultHandler, CancellationToken, ValueTask> resfunc, 
            Func<ITypeReader, CancellationToken, ValueTask> typfunc)
        {
            CommandResultDelegate = cmdfunc;
            CommandRegistrationDelegate = cmdregfunc;
            HandlerRegistrationDelegate = resfunc;
            ReaderRegistrationDelegate = typfunc;
        }

        public override async ValueTask OnCommandExecutedAsync(IContext context, IResult result, CancellationToken cancellationToken)
        {
            if (!(CommandResultDelegate is null))
                await CommandResultDelegate(context, result, cancellationToken).ConfigureAwait(false);
        }

        public override async ValueTask OnCommandRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken)
        {
            if (!(CommandResultDelegate is null))
                await CommandRegistrationDelegate(component, cancellationToken).ConfigureAwait(false);
        }

        public override async ValueTask OnResultHandlerRegisteredAsync(IResultHandler resultHandler, CancellationToken cancellationToken)
        {
            if (!(HandlerRegistrationDelegate is null))
                await HandlerRegistrationDelegate(resultHandler, cancellationToken);
        }

        public override async ValueTask OnTypeReaderRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken)
        {
            if (!(ReaderRegistrationDelegate is null))
                await ReaderRegistrationDelegate(typeReader, cancellationToken);
        }
    }
}
