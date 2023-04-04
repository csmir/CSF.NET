using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.ComponentModel;

namespace CSF
{
    /// <summary>
    ///     Represents helper methods for the <see cref="CommandManager"/>.
    /// </summary>
    public static class ManagerHelper
    {
        /// <summary>
        ///     Adds the <see cref="CommandManager"/> to the <see cref="IServiceCollection"/> and configures it with it's required child components.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="action">A configuration action to set up the manager builder context.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        public static IServiceCollection AddCommandManager(this IServiceCollection collection, Action<ManagerBuilderContext> action = null)
        {
            collection.AddCommandManager<CommandManager>(action);

            return collection;
        }

        /// <summary>
        ///     Adds <typeparamref name="T"/> inheriting <see cref="CommandManager"/> to the <see cref="IServiceCollection"/> and configures it with it's required child components.
        /// </summary>
        /// <typeparam name="T">The type to implement as the command manager for this service provider.</typeparam>
        /// <param name="collection"></param>
        /// <param name="action">A configuration action to set up the manager builder context.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        public static IServiceCollection AddCommandManager<T>(this IServiceCollection collection, Action<ManagerBuilderContext> action = null)
            where T : CommandManager
        {
            var context = new ManagerBuilderContext();

            action?.Invoke(context);

            collection.AddCommandManager<T>(context);

            return collection;
        }

        /// <summary>
        ///     Adds <typeparamref name="T"/> inheriting <see cref="CommandManager"/> to the <see cref="IServiceCollection"/> and configures it with it's required child components.
        /// </summary>
        /// <typeparam name="T">The type to implement as the command manager for this service provider.</typeparam>
        /// <param name="collection"></param>
        /// <param name="context">The manager builder context used to set up the child components and other settings.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection AddCommandManager<T>(this IServiceCollection collection, ManagerBuilderContext context)
            where T : CommandManager
        {
            collection.AddComponents(context);
            collection.AddTypeReaders(context);

            collection.TryAddSingleton<T>();

            return collection;
        }
    }
}
