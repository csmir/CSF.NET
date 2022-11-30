using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default implementation factory. 
    ///     <br/>
    ///     This type <b>can</b> be used when providing the <see cref="CommandFramework{T}"/> with a provider, or overwritten for custom implementations.
    /// </summary>
    public class PipelineProvider : IPipelineProvider
    {
        /// <inheritdoc/>
        public IServiceProvider Provider { get; }

        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; }

        /// <summary>
        ///     Creates a new <see cref="PipelineProvider"/> with default configuration and an empty <see cref="IServiceProvider"/>.
        /// </summary>
        public PipelineProvider()
            : this(new())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="PipelineProvider"/> with defined configuration and an empty <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="configuration">The configuration definition to use in this instance.</param>
        public PipelineProvider(CommandConfiguration configuration)
            : this(configuration, EmptyServiceProvider.Instance)
        {

        }

        /// <summary>
        ///     Creates a new <see cref="PipelineProvider"/> with defined configuration and service provider.
        /// </summary>
        /// <param name="configuration">The configuration definition to use in this instance.</param>
        /// <param name="provider">The service provider to use in this instance.</param>
        public PipelineProvider(CommandConfiguration configuration, IServiceProvider provider)
        {
            Provider = provider;
            Configuration = configuration;
        }

        /// <inheritdoc/>
        public virtual TypeReaderProvider ConfigureTypeReaders()
        {
            var dict = new TypeReaderProvider(TypeReader.CreateDefaultReaders());

            foreach (var reader in AutonomousTypeReaderRegistration())
                dict.Include(reader.Type, reader);

            return dict;
        }

        /// <inheritdoc/>
        public virtual IList<ITypeReader> AutonomousTypeReaderRegistration()
        {
            var list = new List<ITypeReader>();

            var tt = typeof(ITypeReader);

            foreach (var assembly in Configuration.RegistrationAssemblies)
                foreach (var type in assembly.ExportedTypes)
                    if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        list.Add(BuildTypeReader(type));

            return list;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public virtual ResultHandlerProvider ConfigureResultHandlers()
        {
            var dict = new ResultHandlerProvider();

            foreach (var result in AutonomousResultHandlerRegistration())
                dict.Include(result);

            return dict;
        }

        /// <inheritdoc/>
        public virtual IList<IResultHandler> AutonomousResultHandlerRegistration()
        {
            var list = new List<IResultHandler>();

            var tt = typeof(IResultHandler);

            foreach (var assembly in Configuration.RegistrationAssemblies)
                foreach (var type in assembly.ExportedTypes)
                    if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        list.Add(BuildResultHandler(type));

            return list;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public virtual PrefixProvider ConfigurePrefixes()
        {
            var dict = new PrefixProvider();

            foreach (var prefix in AutonomousPrefixRegistration())
                dict.Include(prefix);

            return dict;
        }

        /// <inheritdoc/>
        public virtual IList<IPrefix> AutonomousPrefixRegistration()
        {
            var list = new List<IPrefix>();

            return list;
        }

        /// <inheritdoc/>
        public virtual IPrefix BuildPrefix(Type type)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual ILogger ConfigureLogger()
            => new DefaultLogger(Configuration.DefaultLogLevel);

        /// <inheritdoc/>
        public virtual SearchResult CommandNotFoundResult<TContext>(TContext context)
            where TContext : IContext
            => SearchResult.FromError($"Failed to find command with name: {context.Name}.");

        /// <inheritdoc/>
        public virtual SearchResult NoApplicableOverloadResult<TContext>(TContext context)
            where TContext : IContext
            => SearchResult.FromError($"Failed to find overload that best matches input: {context.Name}.");

        /// <inheritdoc/>
        public virtual ConstructionResult ServiceNotFoundResult<TContext>(TContext context, DependencyInfo dependency)
            where TContext : IContext
            => ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");

        /// <inheritdoc/>
        public virtual ConstructionResult InvalidModuleTypeResult<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
            => ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<TContext>)}");

        /// <inheritdoc/>
        public virtual TypeReaderResult ResolveMissingValue<TContext>(TContext context, ParameterInfo param)
            where TContext : IContext
            => TypeReaderResult.FromSuccess(Type.Missing);

        /// <inheritdoc/>
        public virtual ParseResult MissingOptionalFailedMatch<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext
            => ParseResult.FromError($"Returned type does not match expected result. Expected: '{expectedType.Name}'. Got: '{returnedType.Name}'");

        /// <inheritdoc/>
        public virtual ParseResult OptionalValueNotPopulated<TContext>(TContext context)
            where TContext : IContext
            => ParseResult.FromError($"Optional parameter did not get {nameof(Type.Missing)} or self-populated value.");

        /// <inheritdoc/>
        public virtual ExecuteResult ProcessUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext
            => ExecuteResult.FromError($"Received an unhandled type from method execution: {returnValue.GetType().Name}. \n\rConsider overloading {nameof(ProcessUnhandledReturnType)} if this is intended.");

        /// <inheritdoc/>
        public virtual ExecuteResult UnhandledExceptionResult<TContext>(TContext context, CommandInfo command, Exception ex)
            where TContext : IContext
            => ExecuteResult.FromError(ex.Message, ex);

        /// <summary>
        ///     Disposes managed and unmanaged resources for this <see cref="PipelineProvider"/>.
        /// </summary>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
