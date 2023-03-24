using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a class that's used to describe data from the command.
    /// </summary>
    public class CommandContext : IConsoleContext
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<object> Parameters { get; set; }

        /// <inheritdoc/>
        public string RawInput { get; set; }

        /// <inheritdoc/>
        public IPrefix Prefix { get; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandContext"/> from the provided raw input.
        /// </summary>
        /// <param name="rawInput">The raw input, modified in length if the prefix is populated.</param>
        /// <param name="prefix">The prefix of the command.</param>
        public CommandContext(ParseResult result)
        {
            Prefix = result.Prefix;
            Parameters = result.Arguments;
            NamedParameters = result.NamedArguments;
            Name = result.Name;
        }
    }
}
