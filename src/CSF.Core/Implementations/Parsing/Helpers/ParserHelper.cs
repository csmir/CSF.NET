using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public static class ParserHelper
    {
        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddParser<T>(this IServiceCollection collection)
            where T : class, IParser
        {
            collection.TryAddSingleton<T>();

            return collection;
        }
    }
}
