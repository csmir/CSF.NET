using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace CSF
{
    public static class ServiceHelper
    {
        public static IServiceCollection AddComponents(this IServiceCollection collection, CommandBuildingConfiguration context)
        {
            var rootType = typeof(ModuleBase);

            foreach (var assembly in context.RegistrationAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        collection.TryAddTransient(type);

            return collection;
        }

        public static IServiceCollection AddCommandManager(this IServiceCollection collection, Action<CommandBuildingConfiguration> action = null)
        {
            collection.AddCommandManager<CommandManager>(action);

            return collection;
        }

        public static IServiceCollection AddCommandManager<T>(this IServiceCollection collection, Action<CommandBuildingConfiguration> action = null)
            where T : CommandManager
        {
            var context = new CommandBuildingConfiguration();

            action?.Invoke(context);

            collection.AddCommandManager<T>(context);

            return collection;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection AddCommandManager<T>(this IServiceCollection collection, CommandBuildingConfiguration configuration)
            where T : CommandManager
        {
            collection.AddComponents(configuration);

            collection.AddSingleton(configuration);

            collection.TryAddSingleton<T>();

            return collection;
        }
    }
}
