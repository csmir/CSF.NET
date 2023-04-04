using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a parser for normal strings of text.
    /// </summary>
    public class TextParser : IParser
    {
        private readonly bool _parseNecessary;

        /// <inheritdoc/>
        public PrefixContainer Prefixes { get; }

        /// <summary>
        ///     Creates a new <see cref="TextParser"/> with provided prefixes.
        /// </summary>
        /// <param name="prefixes">The prefixes to define.</param>
        public TextParser(PrefixContainer prefixes)
        {
            Prefixes = prefixes;

            _parseNecessary = Prefixes.Values.Any();
        }

        /// <summary>
        ///     Fetches the prefix for this parser.
        /// </summary>
        /// <param name="rawInput"></param>
        /// <returns></returns>
        public virtual bool TryGetPrefix(ref string rawInput, out IPrefix prefix)
        {
            if (_parseNecessary)
            {
                foreach (var value in Prefixes.Values)
                {
                    if (rawInput.StartsWith(value.Value))
                    {
                        rawInput = rawInput[value.Value.Length..].TrimStart();
                        prefix = value;
                        return true;
                    }
                }
            }
            prefix = EmptyPrefix.Create();
            return false;
        }

        /// <inheritdoc/>
        public virtual ParseResult Parse(string rawInput)
        {
            if (!TryGetPrefix(ref rawInput, out var prefix))
                return ParseResult.FromError("");

            var range = rawInput.Split(' ');

            var name = "";
            var args = new List<object>();
            var partialArgs = new List<string>();

            var argName = "";
            var namedArgs = new Dictionary<string, object>();

            foreach (var entry in range)
            {
                if (name is "")
                {
                    name = entry;
                    continue;
                }

                if (partialArgs.Any())
                {
                    if (entry.EndsWith("\""))
                    {
                        partialArgs.Add(entry.Replace("\"", ""));

                        if (argName is "")
                            args.Add(string.Join(" ", partialArgs));
                        else
                        {
                            namedArgs.Add(argName, string.Join(" ", partialArgs));
                            argName = "";
                        }

                        partialArgs.Clear();
                        continue;
                    }
                    partialArgs.Add(entry);
                    continue;
                }

                if (entry.StartsWith("\""))
                {
                    if (entry.EndsWith("\""))
                    {
                        if (argName is "")
                            args.Add(entry.Replace("\"", ""));
                        else
                        {
                            namedArgs.Add(argName, entry.Replace("\"", ""));
                            argName = "";
                        }
                    }
                    else
                        partialArgs.Add(entry.Replace("\"", ""));
                    continue;
                }

                if (entry.StartsWith("-"))
                    foreach (var c in entry[1..])
                        namedArgs.Add(c.ToString(), null);

                if (entry.StartsWith("--"))
                {
                    if (!entry.EndsWith(":"))
                        namedArgs.Add(entry[1..], null!);
                    else
                        argName = entry[1..^1];
                    continue;
                }

                args.Add(entry);
            }

            return ParseResult.FromSuccess(name, prefix, args, namedArgs);
        }

        /// <inheritdoc/>
        public virtual ParseResult Parse(object rawInput)
        {
            if (rawInput is string str)
                return Parse(str);
            return ParseResult.FromError("Raw argument type does not match supported input. Consider creating a new parser for the input type.");
        }
    }
}
