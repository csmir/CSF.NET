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

    public static class ParameterFlagsExtensions
    {
        public static ParameterFlags WithNullable(this ParameterFlags flags, bool isNullable = true)
        {
            if (isNullable)
                flags |= ParameterFlags.Nullable;
            return flags;
        }

        public static bool HasNullable(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Nullable);

        public static ParameterFlags WithOptional(this ParameterFlags flags, bool isOptional = true)
        {
            if (isOptional)
                flags |= ParameterFlags.Optional;
            return flags;
        }

        public static bool HasOptional(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Optional);

        public static ParameterFlags WithRemainder(this ParameterFlags flags, bool isRemainder = true)
        {
            if (isRemainder)
                flags |= ParameterFlags.Remainder;
            return flags;
        }

        public static bool HasRemainder(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Remainder);
    }
}
