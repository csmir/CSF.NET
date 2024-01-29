using CSF.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Core
{
    public static class CommandManagerResolver
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

        public static IServiceCollection WithCommands<T>(this IServiceCollection collection, CommandConfiguration configuration)
            where T : CommandManager
        {
            collection.ModulesAddTransient(configuration);

            collection.TryAddSingleton(configuration);
            collection.TryAddSingleton<T>();

            return collection;
        }
    }
}
