using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Base configuration for the <see cref="CommandFramework"/>.
    /// </summary>
    public sealed class FrameworkBuilderContext
    {
        /// <summary>
        ///     If enabled, commands will execute asynchronously, ensuring that sync handlers will not wait out the execution before returning to the source method.
        /// </summary>
        /// <remarks>
        ///     When opting in to asynchronous execution, <see cref="CommandFramework{T}.ExecuteCommandsAsync{TContext}(TContext, IServiceProvider, System.Threading.CancellationToken)"/> will always return success immediately after being invoked.
        ///     <br/>
        ///     Read more about the reason behind this in it's XML documentation.
        /// </remarks>
        public bool DoAsynchronousExecution { get; set; } = false;

        public IEnumerable<IPrefix> Prefixes { get; set; } = Array.Empty<IPrefix>();

        /// <summary>
        ///     The assemblies that should be used for registering commands, typereaders and event resolvers.
        /// </summary>
        public Assembly[] RegistrationAssemblies { get; set; } = new[] { Assembly.GetEntryAssembly() };
    }
}
