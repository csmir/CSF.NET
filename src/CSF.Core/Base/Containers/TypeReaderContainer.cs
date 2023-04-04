using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public sealed class TypeReaderContainer
    {
        /// <summary>
        ///     
        /// </summary>
        public IDictionary<Type, ITypeReader> Values { get; }

        public TypeReaderContainer(IEnumerable<ITypeReader> typeReaders)
            => Values = typeReaders.ToDictionary(x => x.Type, x => x);
    }

    /// <summary>
    ///     
    /// </summary>
    public static class TypeReaderContainerExtensions
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddTypeReaders(this IServiceCollection collection, ManagerBuilderContext context)
        {
            var rootType = typeof(ITypeReader);

            foreach (var assembly in context.RegistrationAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        collection.TryAddSingleton(rootType, type);
                }
            }

            foreach (var reader in TypeReader.CreateDefaultReaders())
                collection.TryAddSingleton(rootType, reader.Type);

            collection.TryAddSingleton<TypeReaderContainer>();

            return collection;
        }
    }
}
