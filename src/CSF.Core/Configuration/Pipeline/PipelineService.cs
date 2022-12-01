using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default implementation factory. 
    ///     <br/>
    ///     This type <b>can</b> be used when providing the <see cref="CommandFramework{T}"/> with a provider, or overwritten for custom implementations.
    /// </summary>
    public class PipelineService : IPipelineService
    {
        /// <inheritdoc/>
        public virtual async Task<string> GetInputAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return Console.ReadLine();
        }

        /// <inheritdoc/>
        public virtual async Task<IContext> BuildContextAsync(string rawInput, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new CommandContext(rawInput);
        }

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
    }
}
