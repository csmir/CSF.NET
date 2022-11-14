using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a class that's used to describe data from the command.
    /// </summary>
    public class CommandContext : IContext
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<object> Parameters { get; set; }

        /// <remarks>
        ///     The raw input of the command.
        /// </remarks>
        public string RawInput { get; set; }

        /// <summary>
        ///     The prefix for the command.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if not set in <see cref="CommandContext(string, IPrefix)"/>.
        /// </remarks>
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

            Parameters = param.Item2;
            Name = param.Item1;
        }
    }
}
