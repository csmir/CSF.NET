using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default interface for a command conveyor. This type cannot be used to invoke commands.
    /// </summary>
    public interface ICommandConveyor
    {
        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        public SearchResult OnCommandNotFound<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        public CheckResult OnBestOverloadUnavailable<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        public ConstructionResult OnServiceNotFound<TContext>(TContext context, DependencyParameter dependency)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="ModuleBase"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="ModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        public ConstructionResult OnInvalidModule<TContext>(TContext context, Module module)
            where TContext : IContext;

        /// <summary>
        ///     Called when an optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="BaseParameter.Type"/> or <see cref="Type.Missing"/>.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="param">The parameter that failed to handle.</param>
        /// <returns>A <see cref="TypeReaderResult"/> for the target parameter.</returns>
        public TypeReaderResult OnMissingValue<TContext>(TContext context, IParameterComponent param)
            where TContext : IContext;

        /// <summary>
        ///     Called when the parameter information type is not supported.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="param">The parameter that failed to handle.</param>
        /// <returns>A <see cref="TypeReaderResult"/> for the target parameter.</returns>
        public TypeReaderResult ParameterTypeUnsupported<TContext>(TContext context, IParameterComponent param)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error when <see cref="OnMissingValue{T}(T, BaseParameter)"/> returned a type that did not match the expected type.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="expectedType">The type that was expected to return.</param>
        /// <param name="returnedType">The returned type.</param>
        /// <returns>A <see cref="ArgsResult"/> holding the returned error.</returns>
        public ArgsResult OnMissingReturnedInvalid<TContext>(TContext context, Type expectedType, Type returnedType)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error when <see cref="OnMissingValue{T}(T, BaseParameter)"/> failed to return a valid result. 
        ///     This method has to return <see cref="Type.Missing"/> if no self-implemented value has been returned.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="ArgsResult"/> holding the returned error.</returns>
        public ArgsResult OnOptionalNotPopulated<TContext>(TContext context)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when an unhandled return type has been returned from the command method.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="returnValue">The returned value of the method.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        public ExecuteResult OnUnhandledReturnType<TContext>(TContext context, object returnValue)
            where TContext : IContext;

        /// <summary>
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        public ExecuteResult OnUnhandledException<TContext>(TContext context, Command command, Exception ex)
            where TContext : IContext;

        public ValueTask OnCommandExecuted<TContext>(TContext context, IServiceProvider services, IResult result)
            where TContext : IContext;
    }
}