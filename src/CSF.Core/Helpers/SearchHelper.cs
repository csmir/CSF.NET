namespace CSF
{
    /// <summary>
    ///     Represents helper methods for components.
    /// </summary>
    public static class SearchHelper
    {
        public static CommandCell[] Search(this IEnumerable<IConditionalComponent> components, IContext context, IServiceProvider services)
        {
            var matches = components.Where(command => command.Aliases.Contains(context.Name, StringComparer.InvariantCultureIgnoreCase));

            var cells = matches.Read(context, services);

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

        public static IEnumerable<CommandCell> Read(this IEnumerable<IConditionalComponent> components, IContext context, IServiceProvider services)
        {
            foreach (var component in components)
            {
                if (component is not Command command)
                    continue;

                var length = context.Parameters.Length;
                if (command.MaxLength == length)
                {
                    yield return command.ToCell(context, services);
                }

                if (command.MaxLength <= length)
                {
                    foreach (var parameter in command.Parameters)
                    {
                        if (parameter.IsRemainder)
                        {
                            yield return command.ToCell(context, services);
                        }
                    }
                }

                if (command.MaxLength > length)
                {
                    if (command.MinLength <= length)
                    {
                        yield return command.ToCell(context, services);
                    }
                }
            }
        }

        public static CommandCell ToCell(this Command command, IContext context, IServiceProvider services)
        {
            try
            {
                foreach (var condition in command.Preconditions)
                    condition.Check(context, command, services);

                var arguments = command.Parameters.Parse(context, services);

                return new(command, arguments);
            }
            catch (PipelineException ex)
            {
                return new(ex);
            }
        }

        public static object[] Parse(this IParameterComponent[] parameters, IContext context, IServiceProvider services, int index = 0)
        {
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.IsRemainder)
                {
                    args[i] = parameter.Read(context, services, string.Join(" ", context.Parameters.Skip(index)));
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

                if (parameter.IsNullable && context.Parameters[index] is string str && (str == "null" || str == "nothing"))
                {
                    args[i] = null;
                    index++;
                    continue;
                }

                if (parameter is ComplexParameter complexParam)
                {
                    var result = complexParam.Parameters.Parse(context, services, index);

                    index += result.Length;
                    args[i] = complexParam.Constructor.Target.Invoke(result);
                    continue;
                }

                if (parameter is Parameter basicParam)
                {
                    args[i] = basicParam.Read(context, services, context.Parameters[index]);
                    index++;
                    continue;
                }
            }

            return args;
        }

        public static object Read(this IParameterComponent parameter, IContext context, IServiceProvider services, string value)
            => parameter.TypeReader.Read(context, parameter, services, value);
    }
}
