namespace CSF
{
    public partial class CommandManager
    {
        public virtual CommandCell Search(ICommandContext context, IServiceProvider services)
        {
            var commands = Components.Search(context, services);

            if (commands.Length == 0)
                throw new SearchException("Failed to find any commands that accept the provided input.");

            foreach (var command in commands)
                if (!command.IsInvalid)
                    return command;

            throw new SearchException("Failed to find any commands that accept the provided input.", commands[0].Exception);
        }

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
