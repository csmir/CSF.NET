namespace CSF
{
    /// <summary>
    ///     An attribute that represents the required info to map a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Sets up a new command attribute with the provided name.
        /// </summary>
        /// <param name="name"></param>
        public CommandAttribute(string name)
            : this(name, Array.Empty<string>())
        {

        }

        /// <summary>
        ///     Sets up a new command attribute with the provided name and aliases.
        /// </summary>
        /// <param name="name"></param>
        [CLSCompliant(false)]
        public CommandAttribute(string name, params string[] aliases)
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
