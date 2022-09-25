using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    public interface ITypeReader
    {
        Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo parameter, string value, IServiceProvider provider);
    }
}
