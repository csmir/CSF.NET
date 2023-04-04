using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     
    /// </summary>
    public readonly struct ParseInformation
    {
        /// <summary>
        ///     
        /// </summary>
        public IReadOnlyList<object> Parameters { get; }

        /// <summary>
        ///     
        /// </summary>
        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        /// <summary>
        ///     
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        ///     
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <param name="namedParam"></param>
        /// <param name="prefix"></param>
        public ParseInformation(string name, IReadOnlyList<object> param, IReadOnlyDictionary<string, object> namedParam, string prefix = null)
        {
            Name = name;
            Parameters = param;
            NamedParameters = namedParam;
            Prefix = prefix;
        }
    }
}
