using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents helper methods for components.
    /// </summary>
    public static class ComponentHelper
    {
        /// <summary>
        ///     Adds all available <see cref="IModuleBase"/>'s discovered in the provided assemblies as transient services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="context">The building context to receive registration assemblies from.</param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        public static IServiceCollection AddComponents(this IServiceCollection collection, ManagerBuilderContext context)
        {
            var rootType = typeof(IModuleBase);

            var modules = new List<Type>();
            foreach (var assembly in context.RegistrationAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (rootType.IsAssignableFrom(type) && !type.IsAbstract && !type.ContainsGenericParameters)
                    {
                        modules.Add(type);
                        collection.TryAddTransient(type);
                    }
                }
            }

            var container = new ComponentContainer(modules);

            collection.TryAddSingleton(container);

            return collection;
        }

        /// <summary>
        ///     Casts all values to the target that match <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to cast and match.</typeparam>
        /// <param name="input">The enumerable to cast.</param>
        /// <returns>A new enumerable containing only the values that matched <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> CastWhere<T>(this IEnumerable input)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    yield return @out;
        }

        /// <summary>
        ///     Gets a range of values from an <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The target type of the list.</typeparam>
        /// <param name="input">The list to get a range from.</param>
        /// <param name="startIndex">The start index to start fetching values from.</param>
        /// <param name="count">The amount of values to get.</param>
        /// <returns>A new <see cref="IReadOnlyList{T}"/> containing the selected range.</returns>
        public static IReadOnlyList<T> GetRange<T>(this IReadOnlyList<T> input, int startIndex, int? count = null)
        {
            IEnumerable<T> InnerGetRange()
            {
                count ??= (input.Count - startIndex);

                for (int i = startIndex; i <= count; i++)
                    yield return input[i];
            }

            var range = InnerGetRange();

            return range.ToList();
        }
    }
}
