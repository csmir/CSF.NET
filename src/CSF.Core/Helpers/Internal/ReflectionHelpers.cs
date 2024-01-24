using System.Reflection;

namespace CSF
{
    internal static class ReflectionHelpers
    {
        private static IEnumerable<Module> GetModules(Module module, IDictionary<Type, TypeReader> typeReaders)
        {
            foreach (var group in module.Type.GetNestedTypes())
            {
                foreach (var attribute in group.GetCustomAttributes(true))
                {
                    if (attribute is GroupAttribute gattribute)
                    {
                        yield return new Module(group, typeReaders, module, gattribute.Name, gattribute.Aliases);
                    }
                }
            }
        }

        private static IEnumerable<Command> GetCommands(Module module, IDictionary<Type, TypeReader> typeReaders)
        {
            foreach (var method in module.Type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(true);

                string[] aliases = [];
                foreach (var attribute in attributes)
                {
                    if (attribute is CommandAttribute cmd)
                    {
                        aliases = cmd.Aliases;
                    }

                    // if set to noReg, dont build the command.
                    if (attribute is DontRegisterAttribute noReg)
                    {
                        continue;
                    }
                }

                if (aliases.Length == 0)
                {
                    continue;
                }

                yield return new Command(module, method, aliases, typeReaders);
            }
        }

        public static IConditionalComponent[] GetComponents(this Module module, IDictionary<Type, TypeReader> typeReaders)
        {
            var commands = (IEnumerable<IConditionalComponent>)GetCommands(module, typeReaders)
                .OrderBy(x => x.Parameters.Length);

            var modules = (IEnumerable<IConditionalComponent>)GetModules(module, typeReaders)
                .OrderBy(x => x.Components.Length);

            return commands.Concat(modules)
                .ToArray();
        }

        public static IParameterComponent[] GetParameters(this MethodBase method, IDictionary<Type, TypeReader> typeReaders)
        {
            var parameters = method.GetParameters();

            var arr = new IParameterComponent[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetCustomAttributes().Any(x => x is ComplexAttribute))
                {
                    arr[i] = new ComplexParameter(parameters[i], typeReaders);
                }
                else
                {
                    arr[i] = new Parameter(parameters[i], typeReaders);
                }
            }

            return arr;
        }

        public static PreconditionAttribute[] GetPreconditions(this Attribute[] attributes)
            => attributes.CastWhere<PreconditionAttribute>().ToArray();

        public static Attribute[] GetAttributes(this ICustomAttributeProvider provider, bool inherit)
            => provider.GetCustomAttributes(inherit).CastWhere<Attribute>().ToArray();

        public static Tuple<int, int> GetLength(this IParameterComponent[] parameters)
        {
            var minLength = 0;
            var maxLength = 0;

            foreach (var parameter in parameters)
            {
                if (parameter is ComplexParameter complexParam)
                {
                    maxLength += complexParam.MaxLength;
                    minLength += complexParam.MinLength;
                }

                if (parameter is Parameter defaultParam)
                {
                    maxLength++;
                    if (!defaultParam.IsOptional)
                        minLength++;
                }
            }

            return new(minLength, maxLength);
        }
    }
}
