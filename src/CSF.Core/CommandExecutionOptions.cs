using Microsoft.Extensions.DependencyInjection;

namespace CSF
{
    /// <summary>
    ///     Represents a set of options that change the execution flow of the command handler.
    /// </summary>
    public class CommandExecutionOptions
    {
        /// <summary>
        ///     Gets or sets if the command execution should be asynchronous.
        /// </summary>
        public bool ExecuteAsynchronously { get; set; } = false;

        /// <summary>
        ///     Gets or sets the service scope to be used in command execution. The services used for executing commands will be defaulted to the globally defined <see cref="IServiceProvider"/> if this property is not set.
        /// </summary>
        public IServiceScope Scope { get; set; } = null;

        /// <summary>
        ///     Gets the default options that are used across all executions that don't provide customized <see cref="CommandExecutionOptions"/>. 
        /// </summary>
        public static CommandExecutionOptions Default
            => new();
    }
}
