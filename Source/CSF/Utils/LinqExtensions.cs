using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace CSF.Utils
{
    internal static class LinqExtensions
    {
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

        public static IEnumerable<TOut> SelectWhere<TOut>(this IEnumerable input, Func<TOut, bool> predicate = null)
        {
            foreach (var @in in input)
            {
                if (@in is TOut @out)
                {
                    if (predicate != null)
                        if (predicate.Invoke(@out))
                            yield return @out;
                    else
                        yield return @out;
                }
            }
        }
    }
}
