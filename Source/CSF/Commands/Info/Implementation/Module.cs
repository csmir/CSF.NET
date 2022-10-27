using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents information about the module this command is executed in.
    /// </summary>
    public sealed class Module : IConditionalComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        /// <summary>
        ///     The type of this module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The constructor used to create an instance of the command type.
        /// </summary>
        public Constructor Constructor { get; }

        internal Module(Type type)
        {
            Type = type;
            Constructor = new Constructor(type);

            Attributes = GetAttributes().ToList();
            Preconditions = GetPreconditions().ToList();

            Name = type.Name;
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attr in Type.GetCustomAttributes(true))
                if (attr is Attribute attribute)
                    yield return attribute;
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions()
        {
            foreach (var attr in Attributes)
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
        }
    }
}
