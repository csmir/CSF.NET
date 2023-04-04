using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public static class CollectionHelper
    {
        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<T> CastWhere<T>(this IEnumerable input)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    yield return @out;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
