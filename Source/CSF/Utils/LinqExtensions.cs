using System.Collections.Generic;
using System.Linq;

namespace CSF.Utils
{
    internal static class LinqExtensions
    {
        public static IReadOnlyList<T> GetRange<T>(this IReadOnlyList<T> input, int startIndex, int? count = null)
        {
            IEnumerable<T> Iterate()
            {
                if (count is null)
                    count = input.Count - startIndex - 1;

                for (int i = startIndex; i < count; i++)
                {
                    yield return input[i];
                }
            }

            return Iterate().ToList();
        }
    }
}
