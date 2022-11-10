using System;
using System.Reflection;

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

        /// <summary>
        ///     If <see cref="AutoRegisterModules"/> is enabled, this value needs to be populated with the target assembly to register modules for.
        /// </summary>
        /// <remarks>
        ///     If this value is not needed, do not populate it. It's default value is <see langword="null"/>.
        ///     <br/>
        ///     The <see cref="CommandFramework"/> will not automatically register modules if this value is <see langword="null"/>, even if <see cref="AutoRegisterModules"/> is <see langword="true"/>.
        /// </remarks>
        public Assembly RegistrationAssembly { get; set; } = null;

        /// <summary>
        ///     Represents the log level at which the <see cref="ILogger"/> is created during the creation of the target <see cref="CommandFramework"/>.
        /// </summary>
        /// <remarks>
        ///     This value can freely be changed by changing the default <see cref="ILogger.LogLevel"/> in the <see cref="CommandFramework.Logger"/> property or other references across execution.
        /// </remarks>
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        ///     All <see cref="ITypeReader"/>'s accessible by the <see cref="CommandFramework"/> this configuration will be passed to.
        /// </summary>
        /// <remarks>
        ///     Chain calls to <see cref="TypeReaderProvider.Include(Type, ITypeReader)"/> an <see cref="TypeReaderProvider.Include{T}(TypeReader{T})"/> to populate the dictionary with your own <see cref="ITypeReader"/>'s.
        /// </remarks>
        public TypeReaderProvider TypeReaders { get; set; } 

        /// <summary>
        ///     All <see cref="IPrefix"/>'s accessible by the <see cref="CommandFramework"/> this configuration will be passed to.
        /// </summary>
        public PrefixProvider Prefixes { get; set; } 
    }
}
