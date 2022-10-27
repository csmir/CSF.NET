using System;

namespace CSF
{
    /// <summary>
    ///     Represents information about a service to be injected into the class constructor.
    /// </summary>
    public sealed class ServiceInfo
    {
        /// <summary>
        ///     The service type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        ///     The parameter flags for this service.
        /// </summary>
        public ParameterFlags Flags { get; }

        internal ServiceInfo(Type type, ParameterFlags flags)
        {
            ServiceType = type;
            Flags = flags;
        }
    }
}
