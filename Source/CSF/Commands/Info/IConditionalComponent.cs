using System;
using System.Collections.Generic;
using System.Text;

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
