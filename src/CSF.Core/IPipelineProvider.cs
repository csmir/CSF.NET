using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default interface for an implementation factory.
    /// </summary>
    public interface IPipelineProvider : IDisposable
    {
        /// <summary>
        ///     The default service provider used in <see cref="BuildTypeReader(Type)"/> and <see cref="BuildResultHandler(Type)"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="CommandFramework{T}.ExecuteCommandAsync{TContext}(TContext, IServiceProvider)"/> does not pass a <see cref="IServiceProvider"/>, this provider will be used instead.
        /// </remarks>
        public IServiceProvider Provider { get; }

        /// <summary>
        ///     The configuration to set up the assemblies and potential values 
        /// </summary>
        public CommandConfiguration Configuration { get; }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderProvider"/> with all <see cref="ITypeReader"/>'s in the default definition and registration assemblies.
        /// </summary>
        /// <returns>A <see cref="TypeReaderProvider"/> with all typereaders added in this method and <see cref="AutonomousTypeReaderRegistration"/>.</returns>
        public TypeReaderProvider ConfigureTypeReaders();

        /// <summary>
        ///     Called when typereaders are automatically registered from the available assemblies.
        /// </summary>
        /// <returns>A list of automatically registered <see cref="ITypeReader"/>'s.</returns>
        public IList<ITypeReader> AutonomousTypeReaderRegistration();

        /// <summary>
        ///     Called when <see cref="AutonomousTypeReaderRegistration"/> finds a type to resolve.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The built <see cref="ITypeReader"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ITypeReader BuildTypeReader(Type type);

        /// <summary>
        ///     Creates a new <see cref="ResultHandlerProvider"/> with all <see cref="IResultHandler"/>'s in the registration assemblies.
        /// </summary>
        /// <returns>A <see cref="ResultHandlerProvider"/> with all typereaders added in this method and <see cref="AutonomousResultHandlerRegistration"/>.</returns>
        public ResultHandlerProvider ConfigureResultHandlers();

        /// <summary>
        ///     Called when result handlers are automatically registered from the available assemblies.
        /// </summary>
        /// <returns>A list of automatically registered <see cref="IResultHandler"/>'s.</returns>
        public IList<IResultHandler> AutonomousResultHandlerRegistration();

        /// <summary>
        ///     Called when <see cref="AutonomousResultHandlerRegistration"/> finds a type to resolve.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The built <see cref="IResultHandler"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IResultHandler BuildResultHandler(Type type);


        /// <summary>
        ///     Creates a new <see cref="PrefixProvider"/> with all <see cref="IPrefix"/>'s in the registration assemblies.
        /// </summary>
        /// <returns>A <see cref="PrefixProvider"/> with all typereaders added in this method and <see cref="AutonomousPrefixRegistration"/>.</returns>
        public PrefixProvider ConfigurePrefixes();

        /// <summary>
        ///     Called when prefixes are automatically registered from the available assemblies.
        /// </summary>
        /// <returns>A list of automatically registered <see cref="IPrefix"/>'s.</returns>
        public IList<IPrefix> AutonomousPrefixRegistration();

        /// <summary>
        ///     Called when <see cref="AutonomousPrefixRegistration"/> finds a type to resolve.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The built <see cref="IPrefix"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IPrefix BuildPrefix(Type type);

        /// <summary>
        ///     Configures the application logger and exposes it within the pipeline.
        /// </summary>
        /// <returns>The configured <see cref="ILogger"/> used in the <see cref="CommandFramework{T}"/>.</returns>
        public ILogger ConfigureLogger();

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        public SearchResult CommandNotFoundResult<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        public SearchResult NoApplicableOverloadResult<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        public ConstructionResult ServiceNotFoundResult<TContext>(TContext context, DependencyInfo dependency)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="IModuleBase"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="IModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        public ConstructionResult InvalidModuleTypeResult<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext;

        /// <summary>
        ///     Called when an optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="ParameterInfo.Type"/> or <see cref="Type.Missing"/>.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> for the target parameter.</returns>
        public TypeReaderResult ResolveMissingValue<TContext>(TContext context, ParameterInfo param)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, ParameterInfo)"/> returned a type that did not match the expected type.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="expectedType">The type that was expected to return.</param>
        /// <param name="returnedType">The returned type.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        public ParseResult MissingOptionalFailedMatch<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, ParameterInfo)"/> failed to return a valid result. 
        ///     This method has to return <see cref="Type.Missing"/> if no self-implemented value has been returned.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        public ParseResult OptionalValueNotPopulated<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when an unhandled return type has been returned from the command method.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="returnValue">The returned value of the method.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        public ExecuteResult ProcessUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        public ExecuteResult UnhandledExceptionResult<TContext>(TContext context, CommandInfo command, Exception ex)
            where TContext : IContext;
    }
}