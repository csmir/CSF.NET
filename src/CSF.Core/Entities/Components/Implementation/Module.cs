namespace CSF
{
    /// <summary>
    ///     Represents information about the module this command is executed in.
    /// </summary>
    public sealed class Module : IConditionalComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string[] Aliases { get; }

        /// <inheritdoc/>
        public Attribute[] Attributes { get; }

        /// <inheritdoc/>
        public PreconditionAttribute[] Preconditions { get; }

        /// <summary>
        ///     The components of this module.
        /// </summary>
        public IConditionalComponent[] Components { get; }

        /// <summary>
        ///     The type of this module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The root module. <see langword="null"/> if not available.
        /// </summary>
        public Module Root { get; }

        public Module(Type type, IDictionary<Type, TypeReader> typeReaders, Module root = null, string expectedName = null, string[] aliases = null)
        {
            var attributes = type.GetAttributes(true);
            var preconditions = attributes.GetPreconditions();

            Root = root;
            Type = type;

            Attributes = attributes;
            Preconditions = preconditions;

            Components = this.Build(typeReaders);

            Name = expectedName ?? type.Name;
            Aliases = aliases ?? new string[] { Name };
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{(Root != null ? $"{Root}." : "")}{(Type.Name != Name ? $"{Type.Name}['{Name}']" : $"{Name}")}";
    }
}
