using System.Collections.Generic;

namespace CSF
{
    /// <summary>
    ///     Represents a component with preconditions available.
    /// </summary>
    public interface IConditionalComponent : IComponent
    {
        /// <summary>
        ///     The preconditions of this component.
        /// </summary>
        IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }
    }
}
