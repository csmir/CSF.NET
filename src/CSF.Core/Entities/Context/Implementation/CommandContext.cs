namespace CSF
{
    /// <summary>
    ///     Represents a class that's used to describe data from the command.
    /// </summary>
    public class CommandContext : IStringContext
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string[] Parameters { get; set; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, string> NamedParameters { get; }

        /// <inheritdoc/>
        public string RawInput { get; set; }

        public CommandContext(string rawInput, Parser parser = null)
        {
            parser ??= Parser.Text;

            var result = parser.Parse(rawInput);

            Name = result.Name;
            Parameters = result.Parameters;
            NamedParameters = result.NamedParameters;

            RawInput = rawInput;
        }
    }
}
