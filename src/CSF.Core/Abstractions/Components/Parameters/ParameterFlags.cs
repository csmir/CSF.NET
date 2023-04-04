using System;

namespace CSF
{
    /// <summary>
    ///     Represents certain flags about a parameter.
    /// </summary>
    [Flags]
    public enum ParameterFlags : byte
    {
        /// <summary>
        ///     Defines a default value.
        /// </summary>
        None = 1,

        /// <summary>
        ///     Defines if a parameter is nullable.
        /// </summary>
        Nullable = 2,

        /// <summary>
        ///     Defines if a parameter is optional.
        /// </summary>
        Optional = 4,

        /// <summary>
        ///     Defines if a parameter is remainder.
        /// </summary>
        Remainder = 8,
    }
}
