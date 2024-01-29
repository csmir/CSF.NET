using System.Collections;
using System.ComponentModel;

namespace CSF.Helpers
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class CollectionHelpers
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> CastWhere<T>(this IEnumerable input)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    yield return @out;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T SelectFirstOrDefault<T>(this IEnumerable input, T defaultValue = default)
        {
            foreach (var @in in input)
                if (@in is T @out)
                    return @out;

            return defaultValue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool Contains<T>(this IEnumerable input, bool allowMultipleMatches)
            where T : Attribute
        {
            var found = false;
            foreach (var entry in input)
            {
                if (entry is T)
                {
                    if (!allowMultipleMatches)
                    {
                        if (!found)
                            found = true;
                        else
                            return false;
                    }
                    else
                    {
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }
    }
}
