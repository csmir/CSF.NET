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

        /// <inheritdoc/>
        public bool HasPreconditions { get; }

        /// <summary>
        ///     The components of this module.
        /// </summary>
        public IConditionalComponent[] Components { get; }

        /// <summary>
        ///     Gets the type of this module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Gets the root module of the current module.
        /// </summary>
        /// <remarks>
        ///     This property is <see langword="null"/> if no root is specified for this module.
        /// </remarks>
        public Module Root { get; }

        internal Module(Type type, IDictionary<Type, TypeReader> typeReaders, Module root = null, string expectedName = null, string[] aliases = null)
        {
            var attributes = type.GetAttributes(true);
            var preconditions = attributes.GetPreconditions();

            Root = root;
            Type = type;

            Attributes = attributes;
            Preconditions = preconditions;
            HasPreconditions = preconditions.Length > 0;

            Components = this.Build(typeReaders);

            Name = expectedName ?? type.Name;
            Aliases = aliases ?? [ Name ];
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{(Root != null ? $"{Root}." : "")}{(Type.Name != Name ? $"{Type.Name}['{Name}']" : $"{Name}")}";
    }
}
