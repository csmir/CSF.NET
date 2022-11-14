using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a class that's used to describe data from the command.
    /// </summary>
    public class CommandContext : ICommandContext
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IReadOnlyList<object> Parameters { get; set; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> Flags { get; }

        /// <inheritdoc/>
        public string RawInput { get; set; }

        /// <inheritdoc/>
        public IPrefix Prefix { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandContext"/> from the provided raw input.
        /// </summary>
        /// <param name="rawInput"></param>
        public CommandContext(string rawInput, IPrefix prefix = null)
        {
            if (prefix is null)
                prefix = EmptyPrefix.Create();

            Prefix = prefix;

            var param = Parser.Parse(rawInput);

            RawInput = rawInput;
            Prefix = prefix;

            Parameters = param.Parameters;
            Flags = param.Flags;
            Name = param.Name;
        }
    }
}
