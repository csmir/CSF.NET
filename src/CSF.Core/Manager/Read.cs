namespace CSF
{
    public partial class CommandManager
    {
        /// <summary>
        ///     Reads the parameters of the current command and returns the parsed input to execute it.
        /// </summary>
        /// <param name="command">The command of which the parameters should be evaluated.</param>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>An array of the values returned by the read parameters within the provided command.</returns>
        protected virtual object[] Read(Command command, ICommandContext context, IServiceProvider services)
        {
            if (command.HasParameters)
                return command.Parameters.Read(context, services);
            return Array.Empty<object>();
        }

        /// <summary>
        ///     Reads the provided range of components and returns the resulting typereader information.
        /// </summary>
        /// <param name="components">The components of which the parameters should be evaluated.</param>
        /// <param name="context">The context containing information about the command input.</param>
        /// <param name="services">The services in scope to execute the pipeline in relation to the provided context.</param>
        /// <returns>An array of the values returned by the read parameters within the provided range.</returns>
        protected virtual object[] Read(IParameterComponent[] components, ICommandContext context, IServiceProvider services)
            => components.Read(context, services);
    }

    internal static class ReadOperations
    {
        public static object[] Read(this IParameterComponent[] parameters, ICommandContext context, IServiceProvider services, int index = 0)
        {
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.IsRemainder)
                {
                    var input = string.Join(" ", context.Parameters.Skip(index));
                    if (parameter.Type == typeof(string))
                        args[i] = input;
                    else
                        args[i] = parameter.Read(context, services, input);

                    break;
                }

                if (parameter.IsOptional && context.Parameters.Length <= index)
                {
                    args[i] = Type.Missing;
                    continue;
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    args[i] = context.Parameters[index];
                    index++;
                    continue;
                }

                if (parameter.IsNullable && context.Parameters[index] is "null" or "nothing")
                {
                    args[i] = null;
                    index++;
                    continue;
                }

                if (parameter is ComplexParameter complex)
                {
                    var result = complex.Parameters.Read(context, services, index);

                    index += result.Length;
                    args[i] = complex.Constructor.Target.Invoke(result);
                    continue;
                }

                args[i] = parameter.Read(context, services, context.Parameters[index]);
                index++;
            }
            return args;
        }

        public static object Read(this IParameterComponent parameter, ICommandContext context, IServiceProvider services, string value)
            => parameter.TypeReader.EvalInternal(context, parameter, services, value);
    }
}
