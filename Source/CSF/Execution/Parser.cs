using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     The default parameter parser for provided command phrases.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        ///     Parses the provided raw input into a list of parameters, including the command name.
        /// </summary>
        /// <remarks>
        ///     This parser will <see langword="not"/> handle any steps in removing the prefix of the input. This needs to be done prior to running the parser.
        /// </remarks>
        /// <param name="rawInput">The input to parse.</param>
        /// <returns></returns>
        public static Tuple<string, List<string>> Parse(string rawInput)
        {
            var range = rawInput.Split(' ');

            string commandName = string.Empty;

            List<string> commandParams = new List<string>();
            List<string> partialParam = new List<string>();

            foreach (var entry in range)
            {
                if (commandName == string.Empty)
                {
                    commandName = entry;
                    continue;
                }

                if (partialParam.Any())
                {
                    if (entry.EndsWith("\""))
                    {
                        partialParam.Add(entry.Replace("\"", ""));
                        commandParams.Add(string.Join(" ", partialParam));
                        partialParam.Clear();
                        continue;
                    }
                    partialParam.Add(entry);
                    continue;
                }

                if (entry.StartsWith("\""))
                {
                    if (entry.EndsWith("\""))
                        commandParams.Add(entry.Replace("\"", ""));
                    else
                        partialParam.Add(entry.Replace("\"", ""));
                    continue;
                }

                commandParams.Add(entry);
            }

            return new Tuple<string, List<string>>(commandName, commandParams);
        }
    }
}
