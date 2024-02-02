﻿using CSF.Preconditions;

namespace CSF.Reflection
{
    /// <summary>
    ///     Reveals information about a conditional component, needing validation in order to become part of execution.
    /// </summary>
    public interface IConditional : INameable
    {
        /// <summary>
        ///     Gets an array of aliases for this component.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     Gets an array of <see cref="PreconditionAttribute"/>'s defined atop this component.
        /// </summary>
        public PreconditionAttribute[] Preconditions { get; }

        /// <summary>
        ///     Gets if this component has zero or more preconditions.
        /// </summary>
        public bool HasPreconditions { get; }
    }
}
