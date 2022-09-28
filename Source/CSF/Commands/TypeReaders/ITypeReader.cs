using System;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents an interface used by <see cref="TypeReader{T}"/> without needing to internally provide the generic parameter.
    /// </summary>
    /// <remarks>
    ///     Do not use this interface to build type readers on. Use <see cref="TypeReader{T}"/> instead.
    /// </remarks>
    public interface ITypeReader
    {
        /// <summary>
        ///     Reads the provided parameter value and tries to parse it into the target type.
        /// </summary>
        /// <param name="context">The <see cref="ICommandContext"/> passed into this pipeline.</param>
        /// <param name="parameter">The parameter to implement.</param>
        /// <param name="value">The string value that will populate this parameter.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used in the current scope.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> with provided error or successful parse.</returns>
        Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo parameter, string value, IServiceProvider provider);
    }
}
