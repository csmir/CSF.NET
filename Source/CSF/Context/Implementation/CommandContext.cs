using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a class that's used to describe data from the command.
    /// </summary>
    public class CommandContext : IContext
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string RawInput { get; }

        /// <inheritdoc/>
        public List<string> Parameters { get; }

        /// <inheritdoc/>
        public ISource Source { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandContext"/> from the provided raw input.
        /// </summary>
        /// <param name="rawInput"></param>
        /// <param name="expectedPrefix"></param>
        public CommandContext(string rawInput, string expectedPrefix = null)
        {
            if (!string.IsNullOrEmpty(expectedPrefix))
                rawInput = rawInput.Substring(expectedPrefix.Length);

            RawInput = rawInput;

            var parameters = GetParameters();

            Name = parameters[0];
            Parameters = parameters.GetRange(1, parameters.Count - 1);

            Source = GetSource();
        }

        /// <summary>
        ///     Populates the <see cref="Source"/> of this context.
        /// </summary>
        /// <returns></returns>
        protected virtual ISource GetSource()
        {
            return new ProcessSource();
        }

        /// <summary>
        ///     Populates the <see cref="Parameters"/> of this context.
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> GetParameters()
        {
            return Parser.Parse(RawInput);
        }
    }
}
