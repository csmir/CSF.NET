using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public sealed class Command : IConditionalComponent, IParameterContainer
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public Attribute[] Attributes { get; }

        /// <inheritdoc/>
        public IPrecondition[] Preconditions { get; }

        /// <inheritdoc/>
        public IParameterComponent[] Parameters { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int MaxLength { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public Module Module { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Target { get; }

        public Command(Module module, MethodInfo method, string[] aliases, IDictionary<Type, ITypeReader> typeReaders)
        {
            var attributes = method.GetAttributes(true);
            var preconditions = attributes.GetPreconditions();
            var parameters = method.BuildParameters(typeReaders);

            var (minLength, maxLength) = parameters.GetLength();

            if (attributes.Contains<RemainderAttribute>(false))
                Assert.IsTrue(parameters[^1].IsRemainder, $"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");

            Target = method;
            Module = module;

            Attributes = module.Attributes.Concat(attributes).ToArray();
            Preconditions = module.Preconditions.Concat(preconditions).ToArray();

            Parameters = parameters;

            Name = aliases[0];
            Aliases = aliases;

            MinLength = minLength;
            MaxLength = maxLength;
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{Module}.{Target.Name}['{Name}']({string.Join<IParameterComponent>(", ", Parameters)})";
    }
}
