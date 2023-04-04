using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public static class ParameterFlagsHelper
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static ParameterFlags WithNullable(this ParameterFlags flags, bool isNullable = true)
        {
            if (isNullable)
                flags |= ParameterFlags.Nullable;
            return flags;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool HasNullable(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Nullable);

        /// <summary>
        ///     
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isOptional"></param>
        /// <returns></returns>
        public static ParameterFlags WithOptional(this ParameterFlags flags, bool isOptional = true)
        {
            if (isOptional)
                flags |= ParameterFlags.Optional;
            return flags;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool HasOptional(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Optional);

        /// <summary>
        ///     
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="isRemainder"></param>
        /// <returns></returns>
        public static ParameterFlags WithRemainder(this ParameterFlags flags, bool isRemainder = true)
        {
            if (isRemainder)
                flags |= ParameterFlags.Remainder;
            return flags;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool HasRemainder(this ParameterFlags flags)
            => flags.HasFlag(ParameterFlags.Remainder);
    }
}
