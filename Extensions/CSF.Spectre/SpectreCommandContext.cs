using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;

namespace CSF.Spectre
{
    public class SpectreCommandContext : CommandContext
    {
        /// <summary>
        ///     The underlying console for this handler.
        /// </summary>
        public IAnsiConsole Console { get; }

        public SpectreCommandContext(string rawInput) : base(rawInput)
        {
            Console = AnsiConsole.Console;
        }
    }
}
