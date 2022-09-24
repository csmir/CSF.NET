using CSF.Commands;
using CSF.Info;
using CSF.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSF.TypeReaders
{
    public abstract class TypeReader<T> : ITypeReader
    {
        public abstract Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo info, string value, IServiceProvider provider);
    }
}
