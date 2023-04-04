using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public sealed class ManagerBuilderContext
    {
        /// <summary>
        ///     
        /// </summary>
        public bool DoAsynchronousExecution { get; set; } = false;


        /// <summary>
        ///     
        /// </summary>
        public string[] Prefixes { get; set; } = Array.Empty<string>();

        /// <summary>
        ///     
        /// </summary>
        public Assembly[] RegistrationAssemblies { get; set; } = new Assembly[] { Assembly.GetEntryAssembly() };

        /// <summary>
        ///     
        /// </summary>
        public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
