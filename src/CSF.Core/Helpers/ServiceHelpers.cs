using CSF.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Helpers
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class ServiceHelpers
    {
        public static IServiceCollection WithCommands(this IServiceCollection collection, [DisallowNull] Action<CommandConfiguration> action)
        {
            collection.WithCommands<CommandManager>(action);

            return collection;
        }

        public static IServiceCollection WithCommands<T>(this IServiceCollection collection, [DisallowNull] Action<CommandConfiguration> action)
            where T : CommandManager
        {
            var cmdConf = new CommandConfiguration();

            action(cmdConf);

            collection.WithCommands<T>(cmdConf);

            return collection;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection WithCommands<T>(this IServiceCollection collection, CommandConfiguration configuration)
            where T : CommandManager
        {
            collection.AddModules(configuration);

            collection.TryAddSingleton(configuration);
            collection.TryAddSingleton<T>();

            return collection;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection AddModules(this IServiceCollection collection, CommandConfiguration configuration)
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
