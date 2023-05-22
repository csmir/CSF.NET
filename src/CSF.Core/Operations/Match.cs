using System.ComponentModel;

namespace CSF
{
    public partial class CommandManager
    {
        public virtual CommandCell Match(Command command, ICommandContext context, IServiceProvider services)
            => command.Match(context, services);

        public virtual CommandCell[] Match(IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
            => components.Match(context, services)
                         .ToArray();
    }

    internal static class MatchOperations
    {
        public static IEnumerable<CommandCell> Match(this IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
        {
            foreach (var component in components)
            {
                if (component is not Command command)
                    continue;

                var length = context.Parameters.Length;

                if (command.MaxLength == length)
                    yield return command.Match(context, services);

                if (command.MaxLength <= length && command.HasRemainder)
                    yield return command.Match(context, services);

                if (command.MaxLength > length && command.MinLength <= length)
                    yield return command.Match(context, services);
            }
        }

        public static CommandCell Match(this Command command, ICommandContext context, IServiceProvider services)
        {
            try
            {
                command.Check(context, services);

                var arguments = command.HasParameters 
                    ? command.Parameters.Read(context, services) 
                    : Array.Empty<object>();

                return new(command, arguments);
            }
            catch (PipelineException ex)
            {
                return new(ex);
            }
        }
    }
}
