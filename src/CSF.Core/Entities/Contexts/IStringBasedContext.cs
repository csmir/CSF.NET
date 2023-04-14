namespace CSF
{
    /// <summary>
    ///     Represents an abstract context for text commands.
    /// </summary>
    public interface IStringBasedContext : ICommandContext
    {
        /// <remarks>
        ///     The raw input of the command.
        /// </remarks>
        public string RawInput { get; set; }

        /// <summary>
        ///     The flags present on the command input.
        /// </summary>
        public IReadOnlyDictionary<string, string> NamedParameters { get; }
    }
}
