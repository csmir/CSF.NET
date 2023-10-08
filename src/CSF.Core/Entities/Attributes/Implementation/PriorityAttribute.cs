namespace CSF
{
    /// <summary>
    ///     Represents an attribute that can prioritize one result over another when multiple matches were found.
    /// </summary>
    /// <remarks>
    ///     By default, a command has a priority of 0. Higher values take priority, meaning a command with a priority of 1 will execute first if other commands have a priority of 0.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PriorityAttribute : Attribute
    {
        /// <summary>
        ///     Gets the priority of a command, where higher values take priority over lower ones.
        /// </summary>
        public byte Priority { get; }

        /// <summary>
        ///     Creates a new <see cref="PriorityAttribute"/> with provided priority.
        /// </summary>
        /// <param name="priority">The priority of this command, which can be between 0 and 255.</param>
        public PriorityAttribute(byte priority)
        {
            Assert.IsNotNull(priority);

            Priority = priority;
        }
    }
}
