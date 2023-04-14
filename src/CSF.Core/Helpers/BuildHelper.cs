using System.Linq.Expressions;
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

            var rootReader = typeof(ITypeReader);
            foreach (var assembly in context.RegistrationAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootReader.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        typeReaders.Add(type, Activator.CreateInstance(type) as ITypeReader);

            var rootType = typeof(IModuleBase);
            foreach (var assembly in context.RegistrationAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        yield return new Module(type, typeReaders);
        }

        public static IConditionalComponent[] BuildComponents(this Module module, IDictionary<Type, ITypeReader> typeReaders)
        {
            var commands = (IEnumerable<IConditionalComponent>)module.BuildCommands(typeReaders)
                .OrderBy(x => x.Parameters.Length);

            var modules = (IEnumerable<IConditionalComponent>)module.BuildModules(typeReaders)
                .OrderBy(x => x.Components.Length);

            return commands.Concat(modules)
                .ToArray();
        }

        public static IEnumerable<Module> BuildModules(this Module module, IDictionary<Type, ITypeReader> typeReaders)
        {
            foreach (var group in module.Type.GetNestedTypes())
                foreach (var attribute in group.GetCustomAttributes(true))
                    if (attribute is GroupAttribute gattribute)
                        yield return new Module(group, typeReaders, module, gattribute.Name, gattribute.Aliases);
        }

        public static IEnumerable<Command> BuildCommands(this Module module, IDictionary<Type, ITypeReader> typeReaders)
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

        public static IParameterComponent[] BuildParameters(this MethodBase method, IDictionary<Type, ITypeReader> typeReaders)
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

        public static MethodCallExpression BuildDelegateInterior(this Command command)
        {
            var parametersType = typeof(object[]);
            var servicesType = typeof(IServiceProvider);
            var contextType = typeof(IContext);

            var parameters = Expression.Parameter(
                type: parametersType,
                name: "parameters");

            var services = Expression.Parameter(
                type: servicesType,
                name: "services");

            var context = Expression.Parameter(
                type: contextType,
                name: "context");

            var module = Expression.Variable(
                type: command.Module.Type,
                name: "module");

            var instance = Expression.Block(new[] { module },
                Expression.Assign(
                left: module,
                right: Expression.Convert(
                    type: command.Module.Type,
                    expression: Expression.Call(
                        instance: services,
                        method: servicesType.GetMethod(nameof(IServiceProvider.GetService)),
                        arguments: Expression.Constant(command.Module.Type)))),
                Expression.Assign(
                left: Expression.Property(module, command.Module.Type.GetProperty(nameof(ModuleBase.Context), contextType)),
                right: context),
                Expression.Assign(
                left: Expression.Property(module, command.Module.Type.GetProperty(nameof(ModuleBase.Command))),
                right: Expression.Constant(command)),
                module);

            var call = Expression.Call(
                instance: instance,
                method: command.Target,
                arguments: command.Parameters.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), p.ExposedType)));

            return call;
        }

        public static Func<IServiceProvider, IContext, object[], object> BuildTypeDelegate(this Command command)
        {
            var parametersType = typeof(object[]);
            var servicesType = typeof(IServiceProvider);
            var contextType = typeof(IContext);

            var parameters = Expression.Parameter(
                type: parametersType,
                name: "parameters");

            var services = Expression.Parameter(
                type: servicesType,
                name: "services");

            var context = Expression.Parameter(
                type: contextType,
                name: "context");

            var module = Expression.Variable(
                type: command.Module.Type,
                name: "module");

            var instance = Expression.Block(new[] { module },
                Expression.Assign(
                left: module,
                right: Expression.Convert(
                    type: command.Module.Type,
                    expression: Expression.Call(
                        instance: services,
                        method: servicesType.GetMethod(nameof(IServiceProvider.GetService)),
                        arguments: Expression.Constant(command.Module.Type)))),
                Expression.Assign(
                left: Expression.Property(module, command.Module.Type.GetProperty(nameof(ModuleBase.Context), contextType)),
                right: context),
                Expression.Assign(
                left: Expression.Property(module, command.Module.Type.GetProperty(nameof(ModuleBase.Command))),
                right: Expression.Constant(command)),
                module);

            var call = Expression.Call(
                instance: instance,
                method: command.Target,
                arguments: command.Parameters.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), p.ExposedType)));

            var lambda = Expression.Lambda<Func<IServiceProvider, IContext, object[], object>>(call, services, context, parameters);

            return lambda.Compile();
        }

        public static Action<IServiceProvider, IContext, object[]> BuildVoidDelegate(this Command command)
        {
            var parametersType = typeof(object[]);
            var servicesType = typeof(IServiceProvider);
            var contextType = typeof(IContext);

            var parameters = Expression.Parameter(
                type: parametersType,
                name: "parameters");

            var services = Expression.Parameter(
                type: servicesType,
                name: "services");

            var context = Expression.Parameter(
                type: contextType,
                name: "context");

            var module = Expression.Variable(
                type: command.Module.Type,
                name: "module");

            var instance = Expression.Block(new[] { module },
                Expression.Assign(
                left: module,
                right: Expression.Convert(
                    type: command.Module.Type,
                    expression: Expression.Call(
                        instance: services,
                        method: servicesType.GetMethod(nameof(IServiceProvider.GetService)),
                        arguments: Expression.Constant(command.Module.Type)))),
                Expression.Assign(
                left: Expression.Property(module, command.Module.Type.GetProperty(nameof(ModuleBase.Context), contextType)),
                right: context),
                Expression.Assign(
                left: Expression.Property(module, command.Module.Type.GetProperty(nameof(ModuleBase.Command))),
                right: Expression.Constant(command)),
                module);

            var call = Expression.Call(
                instance: instance,
                method: command.Target,
                arguments: command.Parameters.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), p.ExposedType)));

            var lambda = Expression.Lambda<Action<IServiceProvider, IContext, object[]>>(call, services, context, parameters);

            return lambda.Compile();
        }
    }
}
