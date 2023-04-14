namespace CSF
{
    /// <summary>
    ///     Represents a command group, functioning much like subcommands.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GroupAttribute : Attribute
    {
        /// <summary>
        ///     Represents the name of a group.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The aliases of this group.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Creates a new <see cref="GroupAttribute"/> with defined name.
        /// </summary>
        /// <param name="name"></param>
        public GroupAttribute(string name)
            : this(name, Array.Empty<string>())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="GroupAttribute"/> with defined name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="aliases"></param>
        [CLSCompliant(false)]
        public GroupAttribute(string name, params string[] aliases)
        {
            static void Assign(ref string[] arr, string value, int pos)
            {
                Assert.IsNotEmpty(value);
                arr[pos] = value;
                pos++;
            }

            var i = 0;
            var array = new string[aliases.Length + 1];

            Assign(ref array, name, i);

            foreach (var value in aliases)
                Assign(ref array, value, i);

            Name = name;
            Aliases = array;
        }
    }
}
