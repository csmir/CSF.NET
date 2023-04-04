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
        /// <summary>
        ///     Tries to execute a command with provided <see cref="IContext"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="FrameworkBuilderContext.DoAsynchronousExecution"/> is enabled, the <see cref="IResult"/> of this method will always return success.
        ///     Use the <see cref="CommandExecuted"/> event to do post-execution processing.
        ///     <br/><br/>
        ///     If you want to change the order of execution or add extra steps, override <see cref="RunPipelineAsync{T}(T, IServiceProvider)"/>.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public Task<IResult> ExecuteAsync<TContext>(TContext context, IServiceScope scope = null, CancellationToken cancellationToken = default)
            where TContext : IContext;

        /// <summary>
        ///     Invokes the pipeline for provided context.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="provider">The services for this transient execution.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Searches through the command list to find the best matches.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<SearchResult> SearchAsync<TContext>(TContext context, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Searches through a single module to find the best matches.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="module">The module to search commands in.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<SearchResult> SearchModuleAsync<TContext>(TContext context, Module module, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Checks all resolved commands for matches and preconditions.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="commands">The command matches to be used for executing commands.</param>
        /// <param name="provider">The services to be used for handling the preconditions.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<IResult> CheckAsync<TContext>(TContext context, IEnumerable<Command> commands, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Checks the searched commands to see which match best.
        /// </summary>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="commands">The command matches to be used for executing commands.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<CheckResult> CheckMatchesAsync<TContext>(TContext context, IEnumerable<Command> commands, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Checks the preconditions of a single command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to be used for execution.</param>
        /// <param name="provider">The services used to handle the precondition.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Constructs the module to be used for command execution.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to be used for execution.</param>
        /// <param name="provider">The services used to create the module.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<ConstructionResult> ConstructAsync<TContext>(TContext context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Parses the types found in the command parameters from the provided context.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask<IResult> ReadAsync<TContext>(TContext context, Command command, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Parses the types found in the container parameters from the provided context.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to be used for execution.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<IResult> ReadContainerAsync<TContext>(TContext context, int index, IParameterContainer container, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Executes the provided command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the execution flow.
        /// </remarks>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="module">The module to use for command execution.</param>
        /// <param name="parameters">The parsed parameters to be used when executing the method.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        public ValueTask<IResult> ExecuteAsync<TContext>(TContext context, Command command, ModuleBase module, IEnumerable<object> parameters, CancellationToken cancellationToken)
            where TContext : IContext;
    }
}
