namespace CSF
{
    /// <summary>
    ///     Represents a container that holds and handles parameters.
    /// </summary>
    public interface IParameterContainer
    {
        /// <summary>
        ///     The list of parameters for this component.
        /// </summary>
        public IParameterComponent[] Parameters { get; }

        /// <summary>
        ///     The minimum required length to use a command.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     The optimal length to use a command. If remainder is specified, the count will be set to infinity.
        /// </summary>
        public int MaxLength { get; }
    }
}
