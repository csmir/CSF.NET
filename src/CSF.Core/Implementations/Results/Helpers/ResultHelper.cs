using System.Threading.Tasks;

namespace CSF
{
    public static class ResultHelper
    {
        public static ValueTask<T> AsValueTask<T>(this T value)
        {
            return new ValueTask<T>(value);
        }
    }
}
