using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a generic <see cref="TypeReader{T}"/> to use for parsing provided types into the targetted type.
    /// </summary>
    /// <typeparam name="T">The targetted type for this typereader.</typeparam>
    public abstract class TypeReader<T> : ITypeReader
    {
        /// <inheritdoc />
        public abstract Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo info, string value, IServiceProvider provider);
    }
}
