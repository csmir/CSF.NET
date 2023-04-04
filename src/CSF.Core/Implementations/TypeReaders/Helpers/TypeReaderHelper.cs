using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public static class TypeReaderHelper
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

            foreach (var reader in CreateDefaultReaders())
                collection.TryAddSingleton(rootType, reader.Type);

            collection.TryAddSingleton<TypeReaderContainer>();

            return collection;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ITypeReader> CreateDefaultReaders()
        {
            var range = BaseTypeReader.CreateBaseReaders();

            range.Add(new TimeSpanTypeReader());
            range.Add(new ColorTypeReader());

            return range;
        }
    }
}
