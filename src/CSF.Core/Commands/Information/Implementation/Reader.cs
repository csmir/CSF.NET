using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Information about a typereader.
    /// </summary>
    public sealed class Reader : IComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        //// <inheritdoc/>
        public IList<Attribute> Attributes { get; }

        /// <summary>
        ///     The type of this reader.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The constructor used to create an instance of the reader type.
        /// </summary>
        public Constructor Constructor { get; }

        internal Reader(Type type)
        {
            Type = type;
            Name = type.Name;
            Constructor = new Constructor(type);
            Attributes = GetAttributes()
                .ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Reader Build(Type type)
        {
            if (type == typeof(ITypeReader))
            {
                if (type.ContainsGenericParameters)
                    throw new InvalidOperationException("Untyped generic types cannot be automatically registered. Consider defining DontRegisterAttribute and manually adding them.");
            }
            else
                throw new InvalidOperationException($"Provided type is not a valid {nameof(ITypeReader)}.");

            return new Reader(type);
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attr in Type.GetCustomAttributes(true))
                if (attr is Attribute attribute)
                    yield return attribute;
        }
    }
}
