using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    public abstract class ImplementationFactory
    {
        public IServiceProvider Provider { get; }

        public CommandConfiguration Configuration { get; }

        public ImplementationFactory(IServiceProvider provider, CommandConfiguration configuration)
        {
            Provider = provider;
            Configuration = configuration;
        }

        public virtual TypeReaderProvider ConfigureTypeReaders()
        {
            var dict = new TypeReaderProvider(TypeReader.CreateDefaultReaders());

            foreach (var reader in AutonomousTypeReaderRegistration())
                dict.Include(reader.Type, reader);

            return dict;
        }

        public virtual List<ITypeReader> AutonomousTypeReaderRegistration()
        {
            var list = new List<ITypeReader>();

            var tt = typeof(ITypeReader);

            foreach (var assembly in Configuration.RegistrationAssemblies)
                foreach (var type in assembly.ExportedTypes)
                    if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        list.Add(BuildTypeReader(type));

            return list;
        }

        public virtual ITypeReader BuildTypeReader(Type type)
        {
            var reader = TypeReaderInfo.Build(type);

            var output = reader.Construct(Provider);

            if (output is null)
                throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (output is ITypeReader typeReader)
                return typeReader;

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(ITypeReader)}.");
        }

        public virtual ResultHandlerProvider ConfigureResultHandlers()
        {
            var dict = new ResultHandlerProvider();

            foreach (var result in AutonomousResultHandlerRegistration())
                dict.Include(result);

            return dict;
        }

        public virtual List<IResultHandler> AutonomousResultHandlerRegistration()
        {
            var list = new List<IResultHandler>();

            var tt = typeof(IResultHandler);

            foreach (var assembly in Configuration.RegistrationAssemblies)
                foreach (var type in assembly.ExportedTypes)
                    if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        list.Add(BuildResultHandler(type));

            return list;
        }

        public virtual IResultHandler BuildResultHandler(Type type)
        {
            var handler = ResultHandlerInfo.Build(type);

            var output = handler.Construct(Provider);

            if (output is null)
                throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (output is IResultHandler resultHandler)
                return resultHandler;

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(IResultHandler)}.");
        }

        public virtual PrefixProvider ConfigurePrefixes()
        {
            var dict = new PrefixProvider();

            foreach (var prefix in AutonomousPrefixRegistration())
                dict.Include(prefix);

            return dict;
        }

        public virtual List<IPrefix> AutonomousPrefixRegistration()
        {
            var list = new List<IPrefix>();

            return list;
        }

        public virtual IPrefix BuildPrefix(Type type)
        {
            return null;
        }

        /// <summary>
        ///     Configures the application logger and exposes it within the pipeline.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify what will be registered.
        /// </remarks>
        /// <returns></returns>
        public virtual ILogger ConfigureLogger()
        {
            return new DefaultLogger(Configuration.DefaultLogLevel);
        }

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        public virtual SearchResult CommandNotFoundResult<TContext>(TContext context)
            where TContext : IContext
        {
            return SearchResult.FromError($"Failed to find command with name: {context.Name}.");
        }

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        public virtual SearchResult NoApplicableOverloadResult<TContext>(TContext context)
            where TContext : IContext
        {
            return SearchResult.FromError($"Failed to find overload that best matches input: {context.Name}.");
        }

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        public virtual ConstructionResult ServiceNotFoundResult<TContext>(TContext context, DependencyInfo dependency)
            where TContext : IContext
        {
            return ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");
        }

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="IModuleBase"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="IModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        public virtual ConstructionResult InvalidModuleTypeResult<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
        {
            return ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<TContext>)}");
        }

        /// <summary>
        ///     Called when an optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add uses for <see cref="SelfIfNullAttribute"/>'s.
        ///     <br/>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="ParameterInfo.Type"/> or <see cref="Type.Missing"/>.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> for the target parameter.</returns>
        public virtual TypeReaderResult ResolveMissingValue<TContext>(TContext context, ParameterInfo param)
            where TContext : IContext
        {
            return TypeReaderResult.FromSuccess(Type.Missing);
        }

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, ParameterInfo)"/> returned a type that did not match the expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="expectedType">The type that was expected to return.</param>
        /// <param name="returnedType">The returned type.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        public virtual ParseResult MissingOptionalFailedMatch<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext
        {
            return ParseResult.FromError($"Returned type does not match expected result. Expected: '{expectedType.Name}'. Got: '{returnedType.Name}'");
        }

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, ParameterInfo)"/> failed to return a valid result. 
        ///     This method has to return <see cref="Type.Missing"/> if no self-implemented value has been returned.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        public virtual ParseResult OptionalValueNotPopulated<TContext>(TContext context)
            where TContext : IContext
        {
            return ParseResult.FromError($"Optional parameter did not get {nameof(Type.Missing)} or self-populated value.");
        }

        /// <summary>
        ///     Returns the error message when an unhandled return type has been returned from the command method.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="returnValue">The returned value of the method.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        public virtual ExecuteResult ProcessUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext
        {
            return ExecuteResult.FromError($"Received an unhandled type from method execution: {returnValue.GetType().Name}. \n\rConsider overloading {nameof(ProcessUnhandledReturnType)} if this is intended.");
        }

        /// <summary>
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        public virtual ExecuteResult UnhandledExceptionResult<TContext>(TContext context, CommandInfo command, Exception ex)
            where TContext : IContext
        {
            return ExecuteResult.FromError(ex.Message, ex);
        }
    }
}
