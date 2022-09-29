using System;

namespace CSF
{
    /// <summary>
    ///     Base configuration for the <see cref="CommandStandardizationFramework"/>.
    /// </summary>
    public class CommandConfiguration
    {
        /// <summary>
        ///     If enabled, this ensures that <see cref="CommandStandardizationFramework.CommandRegistered"/> 
        ///     will only be invoked when a command that does not match the same aliases or name is added to the command map.
        /// </summary>
        public bool InvokeOnlyNameRegistrations { get; set; } = false;

        /// <summary>
        ///     If enabled, commands will execute asynchronously, ensuring that sync handlers will not wait out the execution before returning to the source method.
        /// </summary>
        /// <remarks>
        ///     When opting in to asynchronous execution, <see cref="CommandStandardizationFramework.ExecuteCommandAsync{T}(T, IServiceProvider)"/> will always return success immediately after being invoked.
        ///     Read more about the reason behind this in it's XML documentation.
        /// </remarks>
        public bool DoAsynchronousExecution { get; set; } = false;
    }
}
