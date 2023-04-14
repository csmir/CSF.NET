using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents information about the primary constructor of a module.
    /// </summary>
    public class Constructor : IComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        //// <inheritdoc/>
        public Attribute[] Attributes { get; }

        /// <summary>
        ///     The constructor entry point.
        /// </summary>
        public ConstructorInfo Target { get; }

        public Constructor(Type type)
        {
            var target = type.GetConstructors()[0];

            Attributes = target.GetAttributes(true);
            Name = target.Name;
            Target = target;
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{Name}";
    }
}