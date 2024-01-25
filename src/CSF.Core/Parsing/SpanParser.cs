using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Parsing
{
    public abstract class SpanParser<T> : Parser<T>
        where T : struct, IEquatable<T>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object[] Parse(T value)
        {
            throw new NotImplementedException();
        }

        public abstract object[] Parse(Span<T> value);
    }
}
