using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Tests
{
    public class TimeOnlyTypeReader : TypeReader<TimeOnly>
    {
        public override ValueTask<TypeReaderResult> ReadAsync(IContext context, BaseParameter info, object value, CancellationToken cancellationToken)
        {
            return Success(TimeOnly.MinValue);  
        }
    }
}
