using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    internal static class LinqExtensions
    {
        public static IEnumerable<T> CastWhere<T>(this IEnumerable input)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    yield return @out;
        }

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
