using CSF.Core;

namespace CSF.Hosting
{
    public interface IActionFactory : IDisposable
    {
        public ValueTask<ICommandContext> CreateContextAsync(CancellationToken cancellationToken);

        public ValueTask<object[]> CreateArgsAsync(CancellationToken cancellationToken);
    }

    public interface IActionFactory<T> : IActionFactory
        where T : ICommandContext
    {
        public new ValueTask<T> CreateContextAsync(CancellationToken cancellationToken); 

        async ValueTask<ICommandContext> IActionFactory.CreateContextAsync(CancellationToken cancellationToken)
        {
            return await CreateContextAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
