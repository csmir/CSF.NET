using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Utils
{
    internal static class LinqExtensions
    {
        public static IEnumerable<T> GetRange<T>(this IReadOnlyList<T> input, int startIndex, int? count = null)
        {
            if (count is null)
                count = input.Count - startIndex - 1;

            for (int i = startIndex; i < count; i++)
            {
                yield return input[i];
            }
        }
    }
}
