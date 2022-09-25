using System;

namespace CSF
{
    /// <summary>
    ///     The interface implemented by all result types.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     True if succesful, False if not.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        ///     The error message. <c>null</c> if not applicable.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        ///     The result's inner exception. <c>null</c> if not applicable.
        /// </summary>
        Exception Exception { get; }
    }
}
