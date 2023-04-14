namespace CSF
{
    /// <summary>
    ///     Represents a component with preconditions available.
    /// </summary>
    public interface IConditionalComponent : IComponent
    {
        /// <summary>
        ///     The aliases of this component.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The preconditions of this component.
        /// </summary>
        public IPrecondition[] Preconditions { get; }
    }
}
