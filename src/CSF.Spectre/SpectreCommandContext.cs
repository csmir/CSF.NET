using Spectre.Console;
using System;

namespace CSF.Spectre
{
    /// <inheritdoc/>
    public class SpectreCommandContext : CommandContext
    {
        /// <summary>
        ///     The underlying console for this handler.
        /// </summary>
        [CLSCompliant(false)]
        public IAnsiConsole Console { get; }

        /// <inheritdoc/>
        public SpectreCommandContext(string rawInput, Parser parser = null)
            : base(rawInput, parser)
        {
            Console = AnsiConsole.Console;
        }
    }
}
