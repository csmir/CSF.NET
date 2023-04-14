namespace CSF
{
    /// <summary>
    ///     Represents a constructor or method parameter.
    /// </summary>
    public interface IParameterComponent : IComponent
    {
        /// <summary>
        ///     The type of the parameter.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Represents the type that is exposed to the runtime directly, potentially being nullable.
        /// </summary>
        public Type ExposedType { get; }

        /// <summary>
        ///     Represents if the parameter is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        ///     Represents if the parameter is optional.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     Represents if the parameter is remainder.
        /// </summary>
        public bool IsRemainder { get; }

        /// <summary>
        ///     The typereader responsible for parsing this type.
        /// </summary>
        public ITypeReader TypeReader { get; }
    }
}
