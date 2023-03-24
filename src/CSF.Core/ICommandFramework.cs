using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        ///     The service provider used to build and run commands.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public IList<IConditionalComponent> Commands { get; }

        /// <summary>
        ///     The configuration used to configure the command framework.
        /// </summary>
        public CommandConfiguration Configuration { get; }

        /// <summary>
        ///     Registers all assemblies and starts listening for commands.
        /// </summary>
        /// <remarks>
        ///     This call is not holding. If you want the application to hold after calling start, consider using <see cref="RunAsync(bool, CancellationToken)"/> instead.
        /// </remarks>
        /// <param name="autoConfigureAssemblies">If all assemblies should be iterated to register typereaders and modules automatically.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task StartAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Registers all assemblies and holds to listen for commands. 
        /// </summary>
        /// <param name="autoConfigureAssemblies">If all assemblies should be iterated to register typereaders and modules automatically.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task RunAsync(bool autoConfigureAssemblies = true, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Cancels all internal loops used in command handling.
        /// </summary>
        /// <remarks>
        ///     This call will cause <see cref="RunAsync(bool, CancellationToken)"/> to quit out of the loop and return to the caller.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Creates a new <see cref="TypeReaderProvider"/> with all <see cref="ITypeReader"/>'s in the default definition and registration assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with no return type.</returns>
        public ValueTask ConfigureTypeReadersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Called when typereaders are automatically registered from the available assemblies.
        /// </summary>
        /// <param name="assembly">The assembly to build.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with no return type.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValueTask BuildTypeReadersAsync(Assembly assembly, CancellationToken cancellationToken);

        /// <summary>
        ///     Called when <see cref="BuildTypeReadersAsync(Assembly, CancellationToken)"/> finds a type to resolve.
        /// </summary>
        /// <param name="type">The type to build.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with no return type.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValueTask BuildTypeReaderAsync(Type type, CancellationToken cancellationToken);

        /// <summary>
        ///     Builds all modules in the provided assemblies in <see cref="CommandConfiguration.RegistrationAssemblies"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with no return type.</returns>
        public ValueTask ConfigureModulesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Builds all modules in the provided <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to build.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with no return type.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValueTask BuildModulesAsync(Assembly assembly, CancellationToken cancellationToken);

        /// <summary>
        ///     Called when <see cref="BuildModulesAsync(Assembly, CancellationToken)"/>
        /// </summary>
        /// <param name="type">The type to build.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with no return type.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValueTask BuildModuleAsync(Type type, CancellationToken cancellationToken);

        /// <summary>
        ///     Tries to execute a command with provided <see cref="IContext"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="CommandConfiguration.DoAsynchronousExecution"/> is enabled, the <see cref="IResult"/> of this method will always return success.
        ///     Use the <see cref="CommandExecuted"/> event to do post-execution processing.
        ///     <br/><br/>
        ///     If you want to change the order of execution or add extra steps, override <see cref="RunPipelineAsync{T}(T, IServiceProvider)"/>.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public Task<IResult> ExecuteCommandsAsync<TContext>(TContext context, IServiceProvider provider = null, CancellationToken cancellationToken = default)
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ValueTask<CheckResult> CheckAsync<TContext>(TContext context, IEnumerable<Command> commands, IServiceProvider provider, CancellationToken cancellationToken)
            where TContext : IContext;

        /// <summary>
        ///     Checks the searched commands to see which match best.
        /// </summary>
        /// <typeparam name="TContext">The context to execute the pipeline for.</typeparam>
        /// <param name="context">The context to execute the pipeline for.</param>
        /// <param name="commands">The command matches to be used for executing commands.</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel this handle.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> with the <see cref="IResult"/> returned by this handle.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ValueTask<IResult> ExecuteAsync<TContext>(TContext context, Command command, ModuleBase module, IEnumerable<object> parameters, CancellationToken cancellationToken)
            where TContext : IContext;
    }
}
