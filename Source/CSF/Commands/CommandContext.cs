using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents a class thats used to describe data from the command.
    /// </summary>
    public class CommandContext : ICommandContext
    {
        public string Name { get; }

        public string RawInput { get; }

        public List<string> Parameters { get; }

        public CommandContext(string rawInput, string expectedPrefix = null)
        {
            if (!string.IsNullOrEmpty(expectedPrefix))
                rawInput = rawInput.Substring(expectedPrefix.Length);

            (string name, var @params) = ParseParameters(rawInput);

            Name = name;
            RawInput = rawInput;
            Parameters = @params;
        }

        private (string, List<string>) ParseParameters(string rawInput)
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

            return (commandName, commandParams);
        }
    }
}
