namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Searches all available components for the most relevant match.
        /// </summary>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>A command cell containing the best possible result for the provided input.</returns>
        /// <exception cref="SearchException">Thrown when no command was found accepting the provided input.</exception>
        protected virtual CommandCell Search(ICommandContext context, IServiceProvider services)
        {
            var commands = Components.Search(context, services);

            if (commands.Length == 0)
                throw new SearchException("Failed to find any commands that accept the provided input.");

            CommandCell? result = null;

            foreach (var command in commands)
                if (!command.IsInvalid)
                {
                    if (!result.HasValue)
                        result = command;

                    if (command.Command?.Priority > result.Value.Command?.Priority)
                        result = command;
                }

            if (!result.HasValue)
                throw commands[0].Exception;

            return result.Value;
        }

        /// <summary>
        ///     Searches a range of modules for the most relevant match.
        /// </summary>
        /// <param name="components">The components to search.</param>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>An array of command cells that best match the provided input.</returns>
        protected virtual CommandCell[] Search(IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
            => components.Search(context, services);
    }

    internal static class SearchOperations
    {
        public static CommandCell[] Search(this IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
        {
            var matches = components.Where(command => command.Aliases.Any(x => x == context.Name));

            var cells = matches.Match(context, services).ToArray();

            if (cells.All(x => x.IsInvalid))
            {
                var module = matches.SelectFirstOrDefault<Module>();

                if (module is null)
                    return cells;

                context.Name = context.Parameters[0];
                context.Parameters = context.Parameters[1..];

                return module.Components.Search(context, services);
            }

            return cells;
        }
    }
}
