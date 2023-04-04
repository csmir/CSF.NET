using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents the information received from a successful <see cref="IParser.TryParse(object, out ParseInformation)"/> operation.
    /// </summary>
    public readonly struct ParseInformation
    {
        /// <summary>
        ///     The parameters found through parsing.
        /// </summary>
        public IReadOnlyList<object> Parameters { get; }

        /// <summary>
        ///     The named parameters found through parsing.
        /// </summary>
        public IReadOnlyDictionary<string, object> NamedParameters { get; }

        /// <summary>
        ///     The command prefix found through parsing.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        ///     The name of the command found through parsing.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Creates a new <see cref="ParseInformation"/>.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="param">The parameters of the command. Even if there are none, this has to be an empty list and not null.</param>
        public ParseInformation(string name, IReadOnlyList<object> param)
            : this(name, param, null)
        {

        }

        /// <summary>
        ///     Creates a new <see cref="ParseInformation"/>.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="param">The parameters of the command. Even if there are none, this has to be an empty list and not null.</param>
        /// <param name="namedParam">The named parameters found in the command. Can be null.</param>
        /// <param name="prefix">The prefix of the command. Can be null.</param>
        public ParseInformation(string name, IReadOnlyList<object> param, IReadOnlyDictionary<string, object> namedParam = null, string prefix = null)
        {
            Name = name;
            Parameters = param;
            NamedParameters = namedParam;
            Prefix = prefix;
        }
    }
}
