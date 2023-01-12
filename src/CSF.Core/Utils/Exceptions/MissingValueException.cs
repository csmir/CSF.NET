using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Thrown when a value was found missing on evaluation.
    /// </summary>
    public sealed class MissingValueException : ArgumentException
    {
        /// <summary>
        ///     Creates a new <see cref="MissingValueException"/> with provided values.
        /// </summary>
        /// <param name="message">The error message that occurred from this exception.</param>
        /// <param name="paramName">The parameter name that was found to have missing values.</param>
        /// <param name="innerException">The inner exception that led to this exception.</param>
        public MissingValueException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException)
        {

        }

        /// <summary>
        ///     Creates a new <see cref="MissingValueException"/> with provided values.
        /// </summary>
        /// <param name="message">The error message that occurred from this exception.</param>
        /// <param name="paramName">The parameter name that was found to have missing values.</param>
        public MissingValueException(string message, string paramName) 
            : base(paramName, message)
        {

        }
    }
}
