﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public sealed class ComponentContainer
    {
        /// <summary>
        ///     
        /// </summary>
        public IEnumerable<IConditionalComponent> Values { get; }

        public ComponentContainer(IEnumerable<Type> types)
            => Values = types.SelectMany(x => new Module(x).Components);
    }
    
    /// <summary>
    ///     
    /// </summary>
    public static class ComponentContainerExtensions
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceCollection AddComponents(this IServiceCollection collection, ManagerBuilderContext context)
        {
            var rootType = typeof(ModuleBase);

            IEnumerable<Type> YieldModules()
            {
                foreach (var assembly in context.RegistrationAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (rootType.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        {
                            collection.TryAddTransient(rootType, type);
                            yield return type;
                        }
                    }
                }
            }

            var container = new ComponentContainer(YieldModules());

            collection.TryAddSingleton(container);

            return collection;
        }
    }
}
