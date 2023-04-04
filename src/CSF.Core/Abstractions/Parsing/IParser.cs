using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public interface IParser
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="rawInput"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryParse(object rawInput, [NotNullWhen(true)] out ParseInformation result);
    }
}
