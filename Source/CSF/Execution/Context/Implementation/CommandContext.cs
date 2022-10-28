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

        /// <remarks>
        ///     The raw input of the command.
        /// </remarks>
        public string RawInput { get; }

        /// <inheritdoc/>
        public IReadOnlyList<object> Parameters { get; }

        /// <inheritdoc/>
        public ISource Source { get; }

        /// <summary>
        ///     The prefix for the command.
        /// </summary>
        /// <remarks>
        ///     <see langword="null"/> if not set in <see cref="CommandContext(string, IPrefix)"/>.
        /// </remarks>
        public IPrefix Prefix { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandContext"/> from the provided raw input.
        /// </summary>
        /// <param name="rawInput"></param>
        public CommandContext(string rawInput, IPrefix prefix = null)
        {
            RawInput = rawInput;
            Prefix = prefix;

            Parameters = GetParameters();
            Source = GetSource();

            Name = Parameters[0].ToString();
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
        protected virtual IReadOnlyList<object> GetParameters()
        {
            return Parser.Parse(RawInput);
        }
    }
}
