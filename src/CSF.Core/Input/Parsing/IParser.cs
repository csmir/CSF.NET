using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Represents a parser that parses command input into the expected outcome.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        ///     Parses the command input into a new <see cref="ParserOutput"/>.
        /// </summary>
        /// <param name="rawInput">The command input.</param>
        /// <returns>A new <see cref="ParserOutput"/> from the provided values.</returns>
        public ParseResult Parse(string rawInput);

        /// <summary>
        ///     Parses the command input into a new <see cref="ParserOutput"/>.
        /// </summary>
        /// <param name="rawInput">The command input.</param>
        /// <returns>A new <see cref="ParserOutput"/> from the provided values.</returns>
        public ParseResult Parse(object rawInput);
    }
}
