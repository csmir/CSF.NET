using Microsoft.Extensions.DependencyInjection;

namespace CSF
{
    public interface IExecutionOptions
    {
        /// <summary>
        ///     Gets or sets the service scope to be used in command execution. The services used for executing commands will be defaulted to the globally defined <see cref="IServiceProvider"/> if this property is not set.
        /// </summary>
        public IServiceScope Scope { get; set; }
    }

    /// <summary>
    ///     Represents a set of options that change the execution flow of the command handler.
    /// </summary>
    public class ExecutionOptions : IExecutionOptions
    {
        /// <summary>
        ///     Gets or sets the service scope to be used in command execution. The services used for executing commands will be defaulted to the globally defined <see cref="IServiceProvider"/> if this property is not set.
        /// </summary>
        public IServiceScope Scope { get; set; } = null;

        public ExecutionOptions(IServiceProvider provider)
        {
            Scope = provider.CreateScope();
        }
    }
}
