namespace CSF
{
    public partial class CommandManager
    {
        public virtual object[] Parse(Command command, IContext context, IServiceProvider services)
            => command.Parameters.Parse(context, services);

        public virtual object[] Parse(IParameterComponent[] components, IContext context, IServiceProvider services)
            => components.Parse(context, services);
    }

    internal static class ParseOperations
    {
        public static object[] Parse(this IParameterComponent[] parameters, IContext context, IServiceProvider services, int index = 0)
        {
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (parameter.IsRemainder)
                {
                    args[i] = parameter.Parse(context, services, string.Join(" ", context.Parameters.Skip(index)));
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
                    args[i] = basicParam.Parse(context, services, context.Parameters[index]);
                    index++;
                    continue;
                }
            }

            return args;
        }

        public static object Parse(this IParameterComponent parameter, IContext context, IServiceProvider services, string value)
            => parameter.TypeReader.Read(context, parameter, services, value);
    }
}
