namespace CSF
{
    /// <summary>
    ///     Represents helper methods for parameters.
    /// </summary>
    public static class ParameterHelper
    {
        /// <summary>
        ///     Configures if a parameter is nullable.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isNullable">The value to append.</param>
        /// <returns>The same <see cref="ParameterFlags"/> for chaining calls.</returns>
        public static ParameterFlags WithNullable(this ParameterFlags flags, bool isNullable = true)
        {
            if (isNullable)
                flags |= ParameterFlags.Nullable;
            return flags;
        }

        /// <summary>
        ///     Checks if a parameter is nullable.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>True if the flags contain it. False if not.</returns>
        public static bool HasNullable(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Nullable);

        /// <summary>
        ///     Configures if a parameter is optional.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isOptional">The value to append.</param>
        /// <returns>The same <see cref="ParameterFlags"/> for chaining calls.</returns>
        public static ParameterFlags WithOptional(this ParameterFlags flags, bool isOptional = true)
        {
            if (isOptional)
                flags |= ParameterFlags.Optional;
            return flags;
        }

        /// <summary>
        ///     Checks if a parameter is optional.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>True if the flags contain it. False if not.</returns>
        public static bool HasOptional(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Optional);

        /// <summary>
        ///     Configures if a parameter is remainder.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isRemainder">The value to append.</param>
        /// <returns>The same <see cref="ParameterFlags"/> for chaining calls.</returns>
        public static ParameterFlags WithRemainder(this ParameterFlags flags, bool isRemainder = true)
        {
            if (isRemainder)
                flags |= ParameterFlags.Remainder;
            return flags;
        }

        /// <summary>
        ///     Checks if a parameter is remainder.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>True if the flags contain it. False if not.</returns>
        public static bool HasRemainder(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Remainder);
    }
}
