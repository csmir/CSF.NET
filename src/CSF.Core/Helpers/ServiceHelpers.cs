using CSF.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CSF.Helpers
{
    public static class ServiceHelpers
    {
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
