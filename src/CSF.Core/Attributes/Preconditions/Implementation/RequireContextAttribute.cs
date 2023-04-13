namespace CSF
{
    /// <summary>
    ///     Represents a precondition that checks if the provided context is valid.
    /// </summary>
    public sealed class RequireContextAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     The context type to compare against.
        /// </summary>
        public Type ContextType { get; }

        /// <summary>
        ///     Compiles a new <see cref="RequireContextAttribute"/> from provided <paramref name="contextType"/>.
        /// </summary>
        /// <param name="contextType"></param>
        public RequireContextAttribute(Type contextType)
        {
            ContextType = contextType;
        }

        public override void Check(IContext context, Command command, IServiceProvider provider)
        {
            var providedType = context.GetType();

            if (providedType != ContextType)
                Fail($"Invalid context was passed into the command. Expected: '{ContextType.FullName}', got '{providedType.FullName}'");
        }
    }
}
