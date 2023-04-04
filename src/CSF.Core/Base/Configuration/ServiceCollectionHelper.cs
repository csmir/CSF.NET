using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSF
{
    public static class ServiceCollectionHelper
    {
        public static IServiceCollection AddTypeReaders(this IServiceCollection collection, Assembly targetAssembly)
        {
            collection.AddSingleton<TypeReaderContainer>();

            var rootType = typeof(ITypeReader);

            foreach (var type in targetAssembly.GetTypes())
            {
                if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    collection.TryAddSingleton(rootType, type);
            }

            return collection;
        }

        public static IServiceCollection AddComponents(this IServiceCollection collection, Assembly targetAssembly)
        {
            collection.AddSingleton<ComponentContainer>();

            var rootType = typeof(ModuleBase);

            foreach (var type in targetAssembly.GetTypes())
            {
                if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                    collection.TryAddTransient(rootType, type);
            }

            return collection;
        }

        public static IServiceCollection AddPrefixes(this IServiceCollection collection, IEnumerable<IPrefix> prefixes)
        {
            var container = new PrefixContainer(prefixes);
            collection.TryAddSingleton(container);

            return collection;
        }

        public static IServiceCollection AddParser(this IServiceCollection collection, Type parserType)
        {
            collection.TryAddSingleton(typeof(IParser), parserType);
            return collection;
        }

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
            collection.AddPrefixes(context.Prefixes);
            collection.AddParser(context.ParserType);

            foreach (var assembly in context.RegistrationAssemblies)
            {
                collection.AddComponents(assembly);
                collection.AddTypeReaders(assembly);
            }

            collection.AddSingleton<ICommandFramework, T>();

            return collection;
        }
    }
}
