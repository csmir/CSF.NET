using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSF
{
    public static class ServiceCollectionHelper
    {
        public static IServiceCollection AddTypeReaders(this IServiceCollection services, Assembly targetAssembly)
        {
            services.AddSingleton<TypeReaderContainer>();

            var rootType = typeof(ITypeReader);

            foreach (var type in targetAssembly.GetTypes())
            {
                if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    services.AddSingleton(rootType, type);
            }

            return services;
        }

        public static IServiceCollection AddComponents(this IServiceCollection collection, Assembly targetAssembly)
        {
            collection.AddSingleton<ComponentContainer>();

            var rootType = typeof(ModuleBase);

            foreach (var type in targetAssembly.GetTypes())
            {
                if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    collection.AddTransient(rootType, type);
            }

            return collection;
        }

        public static IServiceCollection AddPrefixes(this IServiceCollection collection, IEnumerable<IPrefix> prefixes)
        {
            var container = new PrefixContainer(prefixes);
            collection.AddSingleton(container);

            return collection;
        }

        public static IServiceCollection AddParser(this IServiceCollection collection, Type parserType)
        {
            collection.AddSingleton(typeof(IParser), parserType);
            return collection;
        }

        public static IServiceCollection AddCommandFramework(this IServiceCollection collection, Action<FrameworkBuilderContext> action = null)
        {
            collection.AddCommandFramework<CommandConveyor>(action);

            return collection;
        }

        public static IServiceCollection AddCommandFramework<T>(this IServiceCollection collection, Action<FrameworkBuilderContext> action = null)
            where T : class, ICommandConveyor
        {
            var context = new FrameworkBuilderContext();

            action?.Invoke(context);

            collection.AddCommandFramework<T>(context);

            return collection;
        }

        public static IServiceCollection AddCommandFramework<T>(this IServiceCollection collection, FrameworkBuilderContext context)
            where T : class, ICommandConveyor
        {
            collection.AddPrefixes(context.Prefixes);

            foreach (var assembly in context.RegistrationAssemblies)
            {
                collection.AddComponents(assembly);
                collection.AddTypeReaders(assembly);
            }

            collection.AddSingleton<ICommandConveyor, T>();
            collection.AddSingleton<ICommandFramework, CommandFramework<T>>();

            return collection;
        }
    }
}
