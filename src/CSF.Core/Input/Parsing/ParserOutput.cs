using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public readonly struct ParserOutput
    {
        public IReadOnlyList<object> Parameters { get; }

        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        public string Prefix { get; }

        public string Name { get; }

        public ParserOutput(string name, IReadOnlyList<object> param, IReadOnlyDictionary<string, object> namedParam, string prefix = null)
        {
            Name = name;
            Parameters = param;
            NamedParameters = namedParam;
            Prefix = prefix;
        }
    }
}
