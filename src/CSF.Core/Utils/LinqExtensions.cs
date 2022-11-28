using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSF.Utils
{
    internal static class LinqExtensions
    {
        /// <summary>
        ///     An overload to <see cref="List{T}.GetRange(int, int)"/> for the readonly counterpart.
        /// </summary>
        /// <typeparam name="T">The type contained in this list.</typeparam>
        /// <param name="input"></param>
        /// <param name="startIndex">The start index where the new list should start generating.</param>
        /// <param name="count">The amount of items from the start of the list that should be fetched.</param>
        /// <returns>The new range of <see cref="IReadOnlyList{T}"/></returns>
        public static IReadOnlyList<T> GetRange<T>(this IReadOnlyList<T> input, int startIndex, int? count = null)
        {
            IEnumerable<T> InnerGetRange()
            {
                if (count is null)
                    count = input.Count - startIndex;

                for (int i = startIndex; i <= count; i++)
                    yield return input[i];
            }

            var range = InnerGetRange();

            return range.ToList();
        }

        /// <summary>
        ///     Returns the hard copy range of the initial input <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="TOut">The type to be returned from the range.</typeparam>
        /// <param name="input"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<TOut> SelectWhere<TOut>(this IEnumerable input, Func<TOut, bool> predicate = null)
        {
            foreach (var @in in input)
            {
                if (@in is TOut @out)
                {
                    if (predicate != null)
                    {
                        if (predicate.Invoke(@out))
                            yield return @out;
                    }
                    else
                        yield return @out;
                }
            }
        }
    }
}
