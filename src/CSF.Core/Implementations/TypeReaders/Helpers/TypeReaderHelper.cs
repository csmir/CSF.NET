using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic && !type.ContainsGenericParameters)
                        collection.TryAddSingleton(rootType, type);
                }
            }

            foreach (var reader in CreateDefaultReaders())
                collection.TryAddSingleton(rootType, reader.Type);

            collection.TryAddSingleton<TypeReaderContainer>();

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
