using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            where T : class, ICommandFramework
        {
            var context = new FrameworkBuilderContext();

            action?.Invoke(context);

            collection.AddCommandFramework<T>(context);

            return collection;
        }

        public static IServiceCollection AddCommandFramework<T>(this IServiceCollection collection, FrameworkBuilderContext context)
            where T : class, ICommandFramework
        {
            collection.AddPrefixes(context);
            collection.AddComponents(context);
            collection.AddTypeReaders(context);

            collection.AddSingleton<ICommandFramework, T>();

            return collection;
        }
    }
}
