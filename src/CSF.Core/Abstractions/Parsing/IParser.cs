using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     Represents a command input parser.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        ///     Tries to parse raw input into a valid command input.
        /// </summary>
        /// <param name="rawInput">The raw text to parse.</param>
        /// <param name="result">The information received from parsing the input.</param>
        /// <returns>True if the parse succeeded. False if not.</returns>
        public bool TryParse(object rawInput, [NotNullWhen(true)] out ParseInformation result);
    }
}
