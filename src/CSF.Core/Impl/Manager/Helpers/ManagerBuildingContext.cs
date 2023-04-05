using System.Collections.Generic;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents a context used to set up the command manager and its child containers.
    /// </summary>
    public sealed class ManagerBuilderContext
    {
        /// <summary>
        ///     The assemblies to be used to register
        /// </summary>
        public Assembly[] RegistrationAssemblies { get; set; } = new Assembly[] { Assembly.GetEntryAssembly() };

        /// <summary>
        ///     Metadata that can be passed around the startup logic for further customization of the creation pattern.
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
