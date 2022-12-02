using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    internal class DefaultResultHandler : ResultHandler
    {
        public Func<IContext, IResult, CancellationToken, Task> CommandResultDelegate { get; }

        public Func<IConditionalComponent, CancellationToken, Task> CommandRegistrationDelegate { get; }

        public Func<IResultHandler, CancellationToken, Task> HandlerRegistrationDelegate { get; }

        public Func<ITypeReader, CancellationToken, Task> ReaderRegistrationDelegate { get; }

        internal DefaultResultHandler(
            Func<IContext, IResult, CancellationToken, Task> cmdfunc, 
            Func<IConditionalComponent, CancellationToken, Task> cmdregfunc, 
            Func<IResultHandler, CancellationToken, Task> resfunc, 
            Func<ITypeReader, CancellationToken, Task> typfunc)
        {
            CommandResultDelegate = cmdfunc;
            CommandRegistrationDelegate = cmdregfunc;
            HandlerRegistrationDelegate = resfunc;
            ReaderRegistrationDelegate = typfunc;
        }

        public override async Task OnCommandExecutedAsync(IContext context, IResult result, CancellationToken cancellationToken)
        {
            await CommandResultDelegate?.Invoke(context, result, cancellationToken);
        }

        public override async Task OnCommandRegisteredAsync(IConditionalComponent component, CancellationToken cancellationToken)
        {
            await CommandRegistrationDelegate?.Invoke(component, cancellationToken);
        }

        public override async Task OnResultHandlerRegisteredAsync(IResultHandler resultHandler, CancellationToken cancellationToken)
        {
            await HandlerRegistrationDelegate?.Invoke(resultHandler, cancellationToken);
        }

        public override async Task OnTypeReaderRegisteredAsync(ITypeReader typeReader, CancellationToken cancellationToken)
        {
            await ReaderRegistrationDelegate?.Invoke(typeReader, cancellationToken);
        }
    }
}
