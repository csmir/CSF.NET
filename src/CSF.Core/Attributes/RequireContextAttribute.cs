namespace CSF
{
    /// <summary>
    ///     Represents a precondition that checks if the provided context is valid.
    /// </summary>
    /// <remarks>
    ///     Compiles a new <see cref="RequireContextAttribute{T}"/> from provided <typeparamref name="T"/>.
    /// </remarks>
    public sealed class RequireContextAttribute<T>() : PreconditionAttribute
    {
        /// <summary>
        ///     The context type to compare against.
        /// </summary>
        public Type ContextType { get; } = typeof(T);

        public override ValueTask<CheckResult> EvaluateAsync(ICommandContext context, Command command)
        {
            var providedType = context.GetType();

            if (providedType != ContextType)
                return ValueTask.FromResult(new CheckResult(new CheckException($"Invalid context was passed into the command. Expected: '{ContextType.FullName}', got '{providedType.FullName}'")));

            return ValueTask.FromResult(new CheckResult());
        }
    }
}
