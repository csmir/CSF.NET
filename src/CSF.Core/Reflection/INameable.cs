namespace CSF.Reflection
{
    /// <summary>
    ///     Represents any part of a command.
    /// </summary>
    public interface INameable
    {
        /// <summary>
        ///     Gets the name of the component in question.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the attribute collection for this component.
        /// </summary>
        public Attribute[] Attributes { get; }
    }
}
