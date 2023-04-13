namespace CSF
{
    /// <summary>
    ///     Represents the information received from a successful <see cref="IParser.TryParse(object, out ParserCell)"/> operation.
    /// </summary>
    public readonly struct ParserCell
    {
        /// <summary>
        ///     The name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The parameters found through parsing.
        /// </summary>
        public string[] Parameters { get; }

        /// <summary>
        ///     The named parameters found through parsing.
        /// </summary>
        public IReadOnlyDictionary<string, string> NamedParameters { get; }

        public ParserCell(string[] param, IReadOnlyDictionary<string, string> namedParam = null)
        {
            Parameters = param[1..];
            NamedParameters = namedParam;

            Name = param[0];
        }
    }
}
