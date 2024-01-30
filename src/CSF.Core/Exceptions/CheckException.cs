﻿namespace CSF.Exceptions
{
    /// <summary>
    ///     Represents an <see cref="ExecutionException"/> that is thrown when a command failed precondition validation.
    /// </summary>
    /// <param name="message">The message that represents the reason of the exception being thrown.</param>
    /// <param name="innerException">An exception thrown by an inner operation, if present.</param>
    public sealed class CheckException(string message, Exception innerException = null)
        : ExecutionException(message, innerException)
    {
    }
}
