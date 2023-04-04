using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public static class ManagerHelper
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommandManager(this IServiceCollection collection, Action<ManagerBuilderContext> action = null)
        {
            collection.AddCommandManager<CommandManager>(action);

            return collection;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommandManager<T>(this IServiceCollection collection, Action<ManagerBuilderContext> action = null)
            where T : CommandManager
        {
            var context = new ManagerBuilderContext();

            action?.Invoke(context);

            collection.AddCommandManager<T>(context);

            return collection;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
