﻿namespace CSF.Exceptions
{
    /// <summary>
    ///     Represents an <see cref="ExecutionException"/> that is thrown when no matched command succeeded parsing its parameters.
    /// </summary>
    /// <param name="message">The message that represents the reason of the exception being thrown.</param>
    /// <param name="innerException">An exception thrown by an inner operation, if present.</param>
    public sealed class ReadException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {
    }
}
