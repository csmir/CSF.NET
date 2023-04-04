using System.Threading.Tasks;

namespace CSF
{
    internal static class AsyncExtensions
    {
        public static ValueTask<T> AsValueTask<T>(this T value)
        {
            return new ValueTask<T>(value);
        }
    }
}
