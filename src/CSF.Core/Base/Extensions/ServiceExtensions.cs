using Microsoft.Extensions.DependencyInjection;
using System;

namespace CSF
{
    public static class ServiceCollectionHelper
    {
        public static IServiceCollection AddCommandFramework(this IServiceCollection collection, Action<FrameworkBuilderContext> action = null)
        {
            collection.AddCommandFramework<CommandFramework>(action);

            return collection;
        }

        public static IServiceCollection AddCommandFramework<T>(this IServiceCollection collection, Action<FrameworkBuilderContext> action = null)
            where T : CommandFramework
        {
            var context = new FrameworkBuilderContext();

            action?.Invoke(context);

            collection.AddCommandFramework<T>(context);

            return collection;
        }

        public static IServiceCollection AddCommandFramework<T>(this IServiceCollection collection, FrameworkBuilderContext context)
            where T : CommandFramework
        {
            collection.AddComponents(context);
            collection.AddTypeReaders(context);

            collection.AddSingleton<T>();

            return collection;
        }
    }
}
