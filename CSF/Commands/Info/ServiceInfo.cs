using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Info
{
    /// <summary>
    ///     Represents information about a service to be injected into the class constructor.
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        ///     Defines if the service is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        ///     Defines if the service is optional.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     The service type.
        /// </summary>
        public Type ServiceType { get; }

        internal ServiceInfo(Type type, bool isOptional, bool isNullable)
        {
            ServiceType = type;
            IsNullable = isNullable;
            IsOptional = isOptional;
        }
    }
}
