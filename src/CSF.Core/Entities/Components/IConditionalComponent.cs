﻿namespace CSF
{
    /// <summary>
    ///     Represents a component with preconditions available.
    /// </summary>
    public interface IConditionalComponent : IComponent
    {
        /// <summary>
        ///     Gets the aliases of this component.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Gets the preconditions of this component.
        /// </summary>
        public PreconditionAttribute[] Preconditions { get; }

        /// <summary>
        ///     Gets if this component has any preconditions.
        /// </summary>
        public bool HasPreconditions { get; }
    }
}
