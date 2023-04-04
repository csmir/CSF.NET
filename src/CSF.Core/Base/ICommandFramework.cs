using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the root interface of the <see cref="CommandFramework{T}"/>. This type is not to be used in creating your own command framework.
    /// </summary>
    public interface ICommandFramework
    {
        // exec
        public Task<IResult> ExecuteAsync<TContext>(TContext context, IServiceScope scope = null, CancellationToken cancellationToken = default)
            where TContext : IContext;

        // pipeline
        public ValueTask<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        // search
        public ValueTask<SearchResult> SearchAsync<TContext>(TContext context, CancellationToken cancellationToken)
            where TContext : IContext;

        public ValueTask<SearchResult> SearchModuleAsync<TContext>(TContext context, Module module, CancellationToken cancellationToken)
            where TContext : IContext;

        public SearchResult OnCommandNotFound<TContext>(TContext context)
            where TContext : IContext;

        public CheckResult OnBestOverloadUnavailable<TContext>(TContext context)
            where TContext : IContext;

        // check
        public ValueTask<IResult> CheckAsync<TContext>(TContext context, IEnumerable<Command> commands, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        public ValueTask<CheckResult> CheckMatchesAsync<TContext>(TContext context, IEnumerable<Command> commands, CancellationToken cancellationToken)
            where TContext : IContext;

        public ValueTask<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        public ValueTask<IResult> ReadAsync<TContext>(TContext context, Command command, CancellationToken cancellationToken)
            where TContext : IContext;

        public ValueTask<IResult> ReadContainerAsync<TContext>(TContext context, int index, IParameterContainer container, CancellationToken cancellationToken)
            where TContext : IContext;

        // build
        public ValueTask<ConstructionResult> ConstructAsync<TContext>(TContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        // execute
        public ValueTask<IResult> ExecuteAsync<TContext>(TContext context, Command command, ModuleBase module, IEnumerable<object> parameters, CancellationToken cancellationToken)
            where TContext : IContext;

        // post execute
        public ValueTask AfterExecuteAsync<TContext>(TContext context, IServiceProvider services, IResult result)
            where TContext : IContext;
    }
}
