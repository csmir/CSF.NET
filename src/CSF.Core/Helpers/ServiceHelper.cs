using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace CSF
{
    public static class ServiceHelper
    {
        /// <summary>
        ///     Includes <typeparamref name="T"/> inheriting <see cref="CommandManager"/> into the <see cref="IServiceCollection"/> this method is called on.
        /// </summary>
        /// <typeparam name="T">The type inheriting <see cref="CommandManager"/> to include in the collection.</typeparam>
        /// <param name="collection"></param>
        /// <param name="action">The configuration required to set up a new instance of <see cref="CommandManager"/>.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chained calls.</returns>
        public static IServiceCollection WithCommandManager(this IServiceCollection collection, Action<CMBuilder> action = null)
        {
            var context = new CMBuilder();

            action?.Invoke(context);

            collection.AddCommandManager(context);

            return collection;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection AddCommandManager(this IServiceCollection collection, CMBuilder configuration)
        {
            ModulesAddTransient(collection, configuration);

            var implementor = configuration.Build();

            collection.TryAddSingleton(implementor);

            return collection;
        }

        private static IServiceCollection ModulesAddTransient(IServiceCollection collection, CMBuilder configuration)
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
