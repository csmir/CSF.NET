namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Searches all available components for the most 
        /// </summary>
        /// <param name="context">The command context used to search.</param>
        /// <param name="services">The services used to run the search query's typereaders and preconditions</param>
        /// <returns>A command cell containing the best possible result for the provided input.</returns>
        /// <exception cref="SearchException">Thrown when no command was found accepting the provided input.</exception>
        public virtual CommandCell Search(ICommandContext context, IServiceProvider services)
        {
            var commands = Components.Search(context, services);

            if (commands.Length == 0)
                throw new SearchException("Failed to find any commands that accept the provided input.");

            foreach (var command in commands)
                if (!command.IsInvalid)
                    return command;

            throw commands[0].Exception;
        }

        /// <summary>
        ///     Searches a range of modules for the best available match.
        /// </summary>
        /// <param name="components">The components to search.</param>
        /// <param name="context">The command context used to search.</param>
        /// <param name="services">The services used to run the search query's typereaders and preconditions</param>
        /// <returns>An array of command cells that best match the provided input.</returns>
        public virtual CommandCell[] Search(IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
            => components.Search(context, services);
    }

    internal static class SearchOperations
    {
        public static CommandCell[] Search(this IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
        {
            var matches = components.Where(command => command.Aliases.Contains(context.Name, StringComparer.InvariantCultureIgnoreCase));

            var cells = matches.Match(context, services);

            if (!cells.Any(x => !x.IsInvalid))
            {
                var module = matches.SelectFirstOrDefault<Module>();

                if (module is null)
                    return Array.Empty<CommandCell>();

                context.Name = context.Parameters[0];
                context.Parameters = context.Parameters[1..];

                return module.Components.Search(context, services);
            }

            return cells.ToArray();
        }
    }
}
