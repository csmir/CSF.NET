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
        /// <returns>The same <see cref="ParameterType"/> for chaining calls.</returns>
        public static ParameterType WithNullable(this ParameterType flags, bool isNullable = true)
        {
            if (isNullable)
                flags |= ParameterType.Nullable;
            return flags;
        }

        /// <summary>
        ///     Checks if a parameter is nullable.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>True if the flags contain it. False if not.</returns>
        public static bool HasNullable(this ParameterType flags)
            => flags.HasFlag(ParameterType.Nullable);

        /// <summary>
        ///     Configures if a parameter is optional.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isOptional">The value to append.</param>
        /// <returns>The same <see cref="ParameterType"/> for chaining calls.</returns>
        public static ParameterType WithOptional(this ParameterType flags, bool isOptional = true)
        {
            if (isOptional)
                flags |= ParameterType.Optional;
            return flags;
        }

        /// <summary>
        ///     Checks if a parameter is optional.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>True if the flags contain it. False if not.</returns>
        public static bool HasOptional(this ParameterType flags)
            => flags.HasFlag(ParameterType.Optional);

        /// <summary>
        ///     Configures if a parameter is remainder.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isRemainder">The value to append.</param>
        /// <returns>The same <see cref="ParameterType"/> for chaining calls.</returns>
        public static ParameterType WithRemainder(this ParameterType flags, bool isRemainder = true)
        {
            if (isRemainder)
                flags |= ParameterType.Remainder;
            return flags;
        }

        /// <summary>
        ///     Checks if a parameter is remainder.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>True if the flags contain it. False if not.</returns>
        public static bool HasRemainder(this ParameterType flags)
            => flags.HasFlag(ParameterType.Remainder);
    }
}
