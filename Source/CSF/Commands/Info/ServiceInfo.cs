using System;
using System.Collections.Generic;
using System.Text;

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
        ///     Defines if the service is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        ///     Defines if the service is optional.
        /// </summary>
        public bool IsOptional { get; }

        internal ServiceInfo(Type type, bool isOptional, bool isNullable)
        {
            ServiceType = type;
            IsNullable = isNullable;
            IsOptional = isOptional;
        }
    }
}
