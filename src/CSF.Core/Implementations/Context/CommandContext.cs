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
        public string Prefix { get; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandContext"/> from the provided raw input.
        /// </summary>
        /// <param name="parseInfo">The information returned by an <see cref="IParser"/> to use in context creation.</param>
        public CommandContext(ParseInformation parseInfo)
        {
            Prefix = parseInfo.Prefix;
            Parameters = parseInfo.Parameters;
            NamedParameters = parseInfo.NamedParameters;
            Name = parseInfo.Name;
        }
    }
}
