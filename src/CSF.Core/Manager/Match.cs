using System.ComponentModel;

namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Runs through a found commands and attempts to match the amount of parameters to the represented amount in the context.
        /// </summary>
        /// <param name="command">The command </param>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>A resolved match from the provided command, containing failure or result depending on the outcome of the evaluation.</returns>
        protected virtual CommandCell Match(Command command, ICommandContext context, IServiceProvider services)
            => command.Match(context, services);

        /// <summary>
        ///     Runs through a set of found commands and tests them for the availability
        /// </summary>
        /// <param name="components">The range of components to attempt to match.</param>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>All relevant matches to the provided context.</returns>
        protected virtual CommandCell[] Match(IEnumerable<IConditionalComponent> components, ICommandContext context, IServiceProvider services)
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
