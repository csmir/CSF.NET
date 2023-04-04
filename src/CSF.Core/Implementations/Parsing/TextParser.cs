using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a parser for text command input.
    /// </summary>
    public class TextParser : IParser
    {
        private readonly string[] _prefixes;

        /// <summary>
        ///     Creates a new <see cref="TextParser"/> with provided prefixes. None if no prefix is applicable.
        /// </summary>
        /// <param name="prefixes"></param>
        public TextParser(params string[] prefixes)
        {
            _prefixes = prefixes;
        }

        /// <summary>
        ///     Attempts to get a prefix from the raw text input.
        /// </summary>
        /// <param name="rawInput">The raw input to find a prefix for.</param>
        /// <param name="prefix">The prefix if found, otherwise null.</param>
        /// <returns>True if a prefix was found or no prefix is applicable to this input. False if neither is true.</returns>
        public virtual bool TryGetPrefix(ref string rawInput, out string prefix)
        {
            prefix = null;

            if (_prefixes.Any())
            {
                foreach (var value in _prefixes)
                {
                    if (rawInput.StartsWith(value))
                    {
                        rawInput = rawInput[value.Length..].TrimStart();
                        prefix = value;
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public virtual bool TryParse(object rawInput, [NotNullWhen(true)] out ParseInformation result)
        {
            result = default;

            if (rawInput is string str)
                if (TryParse(str, out result))
                    return true;

            return false;
        }

        /// <summary>
        ///     Tries to parse raw input into a valid command input.
        /// </summary>
        /// <param name="rawInput">The raw text to parse.</param>
        /// <param name="result">The information received from parsing the input.</param>
        /// <returns>True if the parse succeeded. False if not.</returns>
        public virtual bool TryParse(string rawInput, [NotNullWhen(true)] out ParseInformation result)
        {
            result = default;

            if (!TryGetPrefix(ref rawInput, out var prefix))
                return false;

            var splitInput = rawInput.Split(' ');

            var name = "";
            var param = new List<object>();
            var partial = new List<string>();

            var paramName = "";
            var namedParam = new Dictionary<string, object>();

            foreach (var part in splitInput)
            {
                if (name is "")
                {
                    name = part;
                    continue;
                }

                if (partial.Any())
                {
                    if (part.EndsWith("\""))
                    {
                        partial.Add(part.Replace("\"", ""));

                        if (paramName is "")
                            param.Add(string.Join(" ", partial));
                        else
                        {
                            namedParam.Add(paramName, string.Join(" ", partial));
                            paramName = "";
                        }

                        partial.Clear();
                        continue;
                    }
                    partial.Add(part);
                    continue;
                }

                if (part.StartsWith("\""))
                {
                    if (part.EndsWith("\""))
                    {
                        if (paramName is "")
                            param.Add(part.Replace("\"", ""));
                        else
                        {
                            namedParam.Add(paramName, part.Replace("\"", ""));
                            paramName = "";
                        }
                    }
                    else
                        partial.Add(part.Replace("\"", ""));
                    continue;
                }

                if (part.StartsWith("-"))
                    foreach (var c in part[1..])
                        namedParam.Add(c.ToString(), null);

                if (part.StartsWith("--"))
                {
                    if (!part.EndsWith(":"))
                        namedParam.Add(part[1..], null!);
                    else
                        paramName = part[1..^1];
                    continue;
                }

                param.Add(part);
            }

            result = new ParseInformation(name, param, namedParam, prefix);

            return true;
        }
    }
}
