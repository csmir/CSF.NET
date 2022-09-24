using System.Collections.Generic;
using System.Linq;

namespace CSF.Commands
{
    /// <summary>
    ///     Represents a class thats used to describe data from the command.
    /// </summary>
    public class CommandContext : ICommandContext
    {
        public string Name { get; }

        public string RawInput { get; }

        public List<string> Parameters { get; }

        private CommandContext(string name, string rawInput, List<string> parameters)
        {
            Name = name;
            RawInput = rawInput;
            Parameters = parameters;
        }

        /// <summary>
        ///     Creates a new <see cref="CommandContext"/> from the input provider.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandContext Create(string command)
        {
            var range = command.Split(' ');

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

                // start checking the command.
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

            return new CommandContext(commandName, command, commandParams);
        }
    }
}
