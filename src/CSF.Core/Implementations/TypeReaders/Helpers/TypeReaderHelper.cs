using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents helper methods for typereaders.
    /// </summary>
    public static class TypeReaderHelper
    {
        /// <summary>
        ///     Adds all available <see cref="ITypeReader"/>'s discovered in the provided assemblies as singleton services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="context">The building context to receive registration assemblies from.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        public static IServiceCollection AddTypeReaders(this IServiceCollection collection, ManagerBuilderContext context)
        {
            var rootType = typeof(ITypeReader);

            foreach (var assembly in context.RegistrationAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                        collection.AddSingleton(rootType, type);
                }
            }

            foreach (var reader in CreateDefaultReaders())
                collection.AddSingleton(rootType, reader.GetType());

            collection.TryAddSingleton<TypeReaderContainer>();

            return collection;
        }

        /// <summary>
        ///     Adds an <see cref="ITypeReader"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ITypeReader"/> implementation to register.</typeparam>
        /// <param name="collection"></param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        public static IServiceCollection AddTypeReader<T>(this IServiceCollection collection)
            where T : class, ITypeReader
        {
            collection.AddSingleton<ITypeReader, T>();

            return collection;
        }

        /// <summary>
        ///     Adds an <see cref="ITypeReader"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="readerType">The <see cref="ITypeReader"/> implementation to register.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        public static IServiceCollection AddTypeReader(this IServiceCollection collection, Type readerType)
        {
            var rootType = typeof(ITypeReader);

            if (rootType.IsAssignableFrom(readerType) && !readerType.IsAbstract)
                collection.AddSingleton(rootType, readerType); 
            
            return collection;
        }

        /// <summary>
        ///     Gets a range of default <see cref="ITypeReader"/>'s.
        /// </summary>
        /// <returns>A range of <see cref="ITypeReader"/>'s that are defined by default.</returns>
        public static IEnumerable<ITypeReader> CreateDefaultReaders()
        {
            var range = BaseTypeReader.CreateBaseReaders();

            range.Add(new TimeSpanTypeReader());
            range.Add(new ColorTypeReader());

            return range;
        }
    }
}
