using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CSF.Helpers
{
    internal static class ServiceHelpers
    {
        public static IServiceCollection AddCommandManager(this IServiceCollection collection, CommandConfiguration configuration)
        {
            collection.ModulesAddTransient(configuration);

            collection.TryAddSingleton(configuration);
            collection.TryAddSingleton<CommandManager>();

            return collection;
        }

        public static IServiceCollection ModulesAddTransient(this IServiceCollection collection, CommandConfiguration configuration)
        {
            var rootType = typeof(ModuleBase);

            foreach (var assembly in configuration.Assemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        collection.TryAddTransient(type);

            return collection;
        }
    }
}
