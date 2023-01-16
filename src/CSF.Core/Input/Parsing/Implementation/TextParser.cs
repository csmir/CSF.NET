using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a parser for normal strings of text.
    /// </summary>
    public class TextParser : IParser
    {
        /// <summary>
        ///     Represents usable prefixes to parse inputs with.
        /// </summary>
        public PrefixProvider Prefixes { get; }

        /// <summary>
        ///     Gets the default prefix used when no prefix matches the provided type.
        /// </summary>
        public IPrefix DefaultPrefix { get; } = EmptyPrefix.Create();

        /// <summary>
        ///     Creates a new <see cref="TextParser"/> without any defined prefixes.
        /// </summary>
        public TextParser()
            : this(new PrefixProvider())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="TextParser"/> with provided prefixes.
        /// </summary>
        /// <param name="prefixes">The prefixes to define.</param>
        public TextParser(PrefixProvider prefixes)
        {
            Prefixes = prefixes;
        }

        /// <summary>
        ///     Fetches the prefix for this parser.
        /// </summary>
        /// <param name="rawInput"></param>
        /// <returns></returns>
        public virtual IPrefix GetPrefix(ref string rawInput)
        {
            if (Prefixes.TryGetPrefix(rawInput, out var prefix))
            {
                rawInput = rawInput[prefix.Value.Length..].TrimStart();
                return prefix;
            }
            return DefaultPrefix;
        }

        /// <inheritdoc/>
        public virtual ParseResult Parse(string rawInput)
        {
            var range = rawInput.Split(' ');
            var prefix = GetPrefix(ref rawInput);

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
            return ParseResult.FromError("Raw argument type does not match desired input.");
        }
    }
}
