namespace CSF
{
    /// <summary>
    ///     Represents a container that holds and handles parameters.
    /// </summary>
    public interface IParameterContainer
    {
        /// <summary>
        ///     Gets a list of parameters for this container.
        /// </summary>
        public IParameterComponent[] Parameters { get; }

        /// <summary>
        ///     Gets if this container contains any parameters or not.
        /// </summary>
        public bool HasParameters { get; }

        /// <summary>
        ///     Gets the minimum required length to use a command.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     Gets the optimal length to use a command. If remainder is specified, the count will be set to infinity.
        /// </summary>
        public int MaxLength { get; }
    }
}
