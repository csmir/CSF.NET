using System;

namespace CSF
{
    /// <summary>
    ///     Represents a constructor or method parameter.
    /// </summary>
    public interface IParameterComponent : IComponent
    {
        /// <summary>
        ///     The type of the parameter.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The flag collection specifying member data about the parameter.
        /// </summary>
        public ParameterType Flags { get; }
    }
}
