using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Parsing
{
    public class StringParser : Parser<string>
    {
        private static readonly SpanParser<char> _spanParser = new CharSpanParser();

        public override object[] Parse(string value)
        {
            
        }
    }
}
