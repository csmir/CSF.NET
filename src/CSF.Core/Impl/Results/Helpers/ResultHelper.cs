using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents helper methods for results.
    /// </summary>
    public static class ResultHelper
    {
        /// <summary>
        ///     Wraps the value of <typeparamref name="T"/> into a <see cref="ValueTask"/>.
        /// </summary>
        /// <typeparam name="T">The value to wrap.</typeparam>
        /// <param name="value">The value to wrap.</param>
        /// <returns>A <see cref="ValueTask"/> wrapping <typeparamref name="T"/>.</returns>
        public static ValueTask<T> AsValueTask<T>(this T value)
        {
            return new(value);
        }
    }
}
