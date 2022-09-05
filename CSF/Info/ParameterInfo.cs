using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Info
{
    /// <summary>
    ///     Represents a single parameter for the module constructor.
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        ///     Defines if the parameter is optional.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     The parameter type.
        /// </summary>
        public Type Type { get; }

        internal ParameterInfo(System.Reflection.ParameterInfo info)
        {
            IsOptional = info.IsOptional;
            Type = info.ParameterType;
        }
    }
}
