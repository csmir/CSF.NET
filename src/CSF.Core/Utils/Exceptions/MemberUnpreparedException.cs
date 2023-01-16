using System;

namespace CSF
{
    /// <summary>
    ///     Thrown when a member is not ready to be executed.
    /// </summary>
    public sealed class MemberUnpreparedException : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="MemberUnpreparedException"/> with base values.
        /// </summary>
        public MemberUnpreparedException()
            : base("Called an unprepared operation in that was not ready to execute.")
        {

        }

        /// <summary>
        ///     Creates a new <see cref="MemberUnpreparedException"/> with base values.
        /// </summary>
        /// <param name="innerException">The inner exception that led to this exception.</param>
        public MemberUnpreparedException(Exception innerException)
            : base("Called an unprepared operation in that was not ready to execute.", innerException)
        {

        }
    }
}
