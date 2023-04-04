using System.Collections.Generic;

namespace CSF
{
    public readonly struct ParseInformation
    {
        public IReadOnlyList<object> Parameters { get; }

        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        public string Prefix { get; }

        public string Name { get; }

        public ParseInformation(string name, IReadOnlyList<object> param, IReadOnlyDictionary<string, object> namedParam, string prefix = null)
        {
            Name = name;
            Parameters = param;
            NamedParameters = namedParam;
            Prefix = prefix;
        }
    }
}
