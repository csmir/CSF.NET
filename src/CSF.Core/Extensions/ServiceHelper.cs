using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace CSF
{
    public static class ServiceHelper
    {
        /// <summary>
        ///     Adds a range of command components to the collection based on the <see cref="CommandBuildingConfiguration"/> passed into this method.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configuration">The building configuration by which commands are searched and registered.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chained calls.</returns>
        public static IServiceCollection AddComponents(this IServiceCollection collection, CommandBuildingConfiguration configuration)
        {
            var rootType = typeof(ModuleBase);

            foreach (var assembly in configuration.RegistrationAssemblies)
                foreach (var type in assembly.GetTypes())
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        collection.TryAddTransient(type);

            return collection;
        }

        /// <summary>
        ///     Includes a <see cref="CommandManager"/> into the <see cref="IServiceCollection"/> this method is called on.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="action">The configuration required to set up a new instance of <see cref="CommandManager"/>.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chained calls.</returns>
        public static IServiceCollection WithCommandManager(this IServiceCollection collection, Action<CommandBuildingConfiguration> action = null)
        {
            collection.WithCommandManager<CommandManager>(action);

            return collection;
        }

        /// <summary>
        ///     Includes <typeparamref name="T"/> inheriting <see cref="CommandManager"/> into the <see cref="IServiceCollection"/> this method is called on.
        /// </summary>
        /// <typeparam name="T">The type inheriting <see cref="CommandManager"/> to include in the collection.</typeparam>
        /// <param name="collection"></param>
        /// <param name="action">The configuration required to set up a new instance of <see cref="CommandManager"/>.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chained calls.</returns>
        public static IServiceCollection WithCommandManager<T>(this IServiceCollection collection, Action<CommandBuildingConfiguration> action = null)
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
