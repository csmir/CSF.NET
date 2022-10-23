using System;

namespace CSF
{
    /// <summary>
    ///     Base configuration for the <see cref="CommandFramework"/>.
    /// </summary>
    public class CommandConfiguration
    {
        /// <summary>
        ///     If enabled, this ensures that <see cref="CommandFramework.CommandRegistered"/> 
        ///     will only be invoked when a command that does not match the same aliases or name is added to the command map.
        /// </summary>
        public bool InvokeOnlyNameRegistrations { get; set; } = false;

        /// <summary>
        ///     If enabled, commands will execute asynchronously, ensuring that sync handlers will not wait out the execution before returning to the source method.
        /// </summary>
        /// <remarks>
        ///     When opting in to asynchronous execution, <see cref="CommandFramework.ExecuteCommandAsync{T}(T, IServiceProvider)"/> will always return success immediately after being invoked.
        ///     <br/>
        ///     Read more about the reason behind this in it's XML documentation.
        /// </remarks>
        public bool DoAsynchronousExecution { get; set; } = false;

        /// <summary>
        ///     If enabled, modules will be registered automatically when a new <see cref="CommandFramework"/> is created.
        /// </summary>
        /// <remarks>
        ///     In some apps or API/BPI's, registration steps require certain values to be configured before commands can be written to them.
        ///     <br/>
        ///     In these cases, it is best to disable this option and self-implement module registration through <see cref="CommandFramework.BuildModulesAsync(System.Reflection.Assembly)"/> to avoid failures or unexpected behavior.
        /// </remarks>
        public bool AutoRegisterModules { get; set; } = false;
    }
}
