namespace CSF
{
    /// <summary>
    ///     Represents the error code that occurred during command execution. <see cref="Success"/> means there was no negative result.
    /// </summary>
    public enum FailureCode : int
    {
        /// <summary>
        ///     Represents no negative result.
        /// </summary>
        Success,

        /// <summary>
        ///     Represents a result tied to <see cref="SearchException"/>. Thrown when no command could be found.
        /// </summary>
        Search,

        /// <summary>
        ///     Represents a result tied to <see cref="CheckException"/>. Thrown when no matched command succeeded its precondition checks.
        /// </summary>
        Check,

        /// <summary>
        ///     Represents a result tied to <see cref="ReadException"/>. Thrown when no matched command succeeded parsing its parameters.
        /// </summary>
        Read,

        /// <summary>
        ///     Represents a result tied to <see cref="ExecuteException"/>. Thrown when the command being executed failed to run its body.
        /// </summary>
        Execute,
    }
}
