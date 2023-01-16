using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF.Parsing
{
    /// <summary>
    ///     Represents a parser for normal strings of text.
    /// </summary>
    public class TextParser : IParser
    {
        /// <inheritdoc/>
        public ArgsResult Parse(string rawInput)
        {
            var range = rawInput.Split(' ');

            var name = "";
            var args = new List<object>();
            var partialArgs = new List<string>();

            var argName = "";
            var namedArgs = new Dictionary<string, object?>();

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

            return ArgsResult.FromSuccess(name, args, namedArgs);
        }

        /// <inheritdoc/>
        public ArgsResult Parse(object rawInput)
        {
            if (rawInput is string str)
                return Parse(str);
            return ArgsResult.FromError("Raw argument type does not match desired input.");
        }
    }
}
