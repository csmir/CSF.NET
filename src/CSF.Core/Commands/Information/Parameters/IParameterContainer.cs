using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public interface IParameterContainer
    {
        /// <summary>
        ///     The list of parameters for this component.
        /// </summary>
        public IReadOnlyCollection<IParameterComponent> Parameters { get; }

        /// <summary>
        ///     The minimum required length to use a command.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     The optimal length to use a command. If remainder is specified, the count will be set to infinity.
        /// </summary>
        public int MaxLength { get; }
    }
}
