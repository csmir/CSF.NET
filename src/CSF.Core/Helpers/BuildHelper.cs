using System.Reflection;

namespace CSF
{
    public static class BuildHelper
    {
        public static IConditionalComponent[] Build(this CommandBuildingConfiguration context)
        {
            var modules = context.BuildModules();

            return modules.SelectMany(x => x.Components).ToArray();
        }

        public static IEnumerable<Module> BuildModules(this CommandBuildingConfiguration context)
        {
            var typeReaders = context.TypeReaders.ToDictionary(x => x.Type, x => x);

            var rootReader = typeof(TypeReader);
            foreach (var assembly in context.RegistrationAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootReader.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        typeReaders.Add(type, Activator.CreateInstance(type) as TypeReader);

            var rootType = typeof(ModuleBase);
            foreach (var assembly in context.RegistrationAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        yield return new Module(type, typeReaders);
        }

        public static IConditionalComponent[] Build(this Module module, IDictionary<Type, TypeReader> typeReaders)
        {
            var commands = (IEnumerable<IConditionalComponent>)module.BuildCommands(typeReaders)
                .OrderBy(x => x.Parameters.Length);

            var modules = (IEnumerable<IConditionalComponent>)module.BuildModules(typeReaders)
                .OrderBy(x => x.Components.Length);

            return commands.Concat(modules)
                .ToArray();
        }

        public static IEnumerable<Module> BuildModules(this Module module, IDictionary<Type, TypeReader> typeReaders)
        {
            foreach (var group in module.Type.GetNestedTypes())
                foreach (var attribute in group.GetCustomAttributes(true))
                    if (attribute is GroupAttribute gattribute)
                        yield return new Module(group, typeReaders, module, gattribute.Name, gattribute.Aliases);
        }

        public static IEnumerable<Command> BuildCommands(this Module module, IDictionary<Type, TypeReader> typeReaders)
        {
            foreach (var method in module.Type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(true);

                string[] aliases = Array.Empty<string>();
                foreach (var attribute in attributes)
                    if (attribute is CommandAttribute commandAttribute)
                        aliases = commandAttribute.Aliases;

                if (!aliases.Any())
                    continue;

                yield return new Command(module, method, aliases, typeReaders);
            }
        }

        public static IParameterComponent[] BuildParameters(this MethodBase method, IDictionary<Type, TypeReader> typeReaders)
        {
            var parameters = method.GetParameters();

            var arr = new IParameterComponent[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].GetCustomAttributes().Any(x => x is ComplexAttribute))
                    arr[i] = new ComplexParameter(parameters[i], typeReaders);
                else
                    arr[i] = new Parameter(parameters[i], typeReaders);
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
