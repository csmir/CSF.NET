using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;

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
        public SpectreCommandContext(string rawInput) : base(rawInput)
        {
            Console = AnsiConsole.Console;
        }
    }
}
