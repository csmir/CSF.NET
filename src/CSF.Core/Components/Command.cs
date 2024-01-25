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
        public PreconditionAttribute[] Preconditions { get; }

        /// <inheritdoc/>
        public bool HasPreconditions { get; }

        /// <inheritdoc/>
        public IParameterComponent[] Parameters { get; }

        /// <inheritdoc/>
        public bool HasParameters { get; }

        /// <inheritdoc/>
        public bool HasRemainder { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int MaxLength { get; }

        /// <inheritdoc/>
        public string[] Aliases { get; }

        /// <summary>
        ///     Represents the priority of a command.
        /// </summary>
        public byte Priority { get; }

        /// <summary>
        ///     Gets the module this command is declared in.
        /// </summary>
        public Module Module { get; }

        /// <summary>
        ///     Gets the target method this command is aimed to execute.
        /// </summary>
        public MethodInfo Target { get; }

        internal Command(Module module, MethodInfo method, string[] aliases, IDictionary<Type, TypeReader> typeReaders)
        {
            var attributes = method.GetAttributes(true);
            var preconditions = attributes.GetPreconditions();
            var parameters = method.GetParameters(typeReaders);

            var (minLength, maxLength) = parameters.GetLength();

            if (parameters.Any(x => x.Attributes.Contains<RemainderAttribute>(false)))
            {
                if (parameters[^1].IsRemainder)
                {
                    ThrowHelpers.InvalidOp($"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");
                }
            }

            Priority = attributes.SelectFirstOrDefault<PriorityAttribute>()?.Priority ?? 0;

            Target = method;
            Module = module;

            Attributes = attributes;
            Preconditions = preconditions;
            HasPreconditions = preconditions.Length > 0;

            Parameters = parameters;
            HasParameters = parameters.Length > 0;
            HasRemainder = parameters.Any(x => x.IsRemainder);

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
