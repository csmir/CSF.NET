using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default command factory. 
    ///     <br/>
    ///     This type <b>can</b> be used when providing the <see cref="CommandFramework{T}"/> with a provider, or overwritten for custom implementations.
    /// </summary>
    public class CommandConveyor : ICommandConveyor
    {
        /// <inheritdoc/>
        public virtual SearchResult OnCommandNotFound<TContext>(TContext context)
            where TContext : IContext
            => SearchResult.FromError($"Failed to find command with name: {context.Name}.");

        /// <inheritdoc/>
        public virtual CheckResult OnBestOverloadUnavailable<TContext>(TContext context)
            where TContext : IContext
            => CheckResult.FromError($"Failed to find overload that best matches input: {context.Name}.");

        /// <inheritdoc/>
        public virtual ConstructionResult OnServiceNotFound<TContext>(TContext context, DependencyParameter dependency)
            where TContext : IContext
            => ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");

        /// <inheritdoc/>
        public virtual ConstructionResult OnInvalidModule<TContext>(TContext context, Module module)
            where TContext : IContext
            => ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<TContext>)}");

        /// <inheritdoc/>
        public virtual TypeReaderResult OnMissingValue<TContext>(TContext context, IParameterComponent param)
            where TContext : IContext
            => TypeReaderResult.FromSuccess(Type.Missing);

        /// <inheritdoc/>
        public virtual TypeReaderResult ParameterTypeUnsupported<TContext>(TContext context, IParameterComponent param)
            where TContext : IContext
            => TypeReaderResult.FromError("Parameter information type is unsupported.");

        /// <inheritdoc/>
        public virtual ArgsResult OnMissingReturnedInvalid<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext
            => ArgsResult.FromError($"Returned type does not match expected result. Expected: '{expectedType.Name}'. Got: '{returnedType.Name}'");

        /// <inheritdoc/>
        public virtual ArgsResult OnOptionalNotPopulated<TContext>(TContext context)
            where TContext : IContext
            => ArgsResult.FromError($"Optional parameter did not get {nameof(Type.Missing)} or self-populated value.");

        /// <inheritdoc/>
        public virtual ExecuteResult OnUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext
            => ExecuteResult.FromError($"Received an unhandled type from method execution: {returnValue.GetType().Name}. \n\rConsider overloading {nameof(OnUnhandledReturnType)} if this is intended.");

        /// <inheritdoc/>
        public virtual ExecuteResult OnUnhandledException<TContext>(TContext context, Command command, Exception ex)
            where TContext : IContext
            => ExecuteResult.FromError(ex.Message, ex);

        public virtual ValueTask OnExecuted<TContext>(TContext context, IServiceProvider services, IResult result)
            where TContext : IContext
            => new ValueTask(Task.CompletedTask);
    }
}
