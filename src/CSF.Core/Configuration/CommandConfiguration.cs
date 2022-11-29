using System;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Base configuration for the <see cref="CommandFramework"/>.
    /// </summary>
    public sealed class CommandConfiguration
    {
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
        ///     Represents the log level at which the <see cref="ILogger"/> is created during the creation of the target <see cref="CommandFramework"/>.
        /// </summary>
        /// <remarks>
        ///     This value can freely be changed by changing the default <see cref="ILogger.LogLevel"/> in the <see cref="CommandFramework.Logger"/> property or other references across execution.
        /// </remarks>
        public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        ///     The assemblies that should be used for registering commands, typereaders and event resolvers.
        /// </summary>
        public Assembly[] RegistrationAssemblies { get; set; } = new[] { Assembly.GetEntryAssembly() };

        /// <summary>
        ///     If assemblies registered in <see cref="RegistrationAssemblies"/> should auto-register.
        /// </summary>
        public bool AutoRegisterAssemblies { get; set; } = true;
    }
}
