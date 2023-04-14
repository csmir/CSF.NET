namespace CSF
{
    /// <summary>
    ///     Represents an interface that supports all implementations of command context classes.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        ///     The command parameters.
        /// </summary>
        public string[] Parameters { get; set; }

        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; set; }
    }
}
