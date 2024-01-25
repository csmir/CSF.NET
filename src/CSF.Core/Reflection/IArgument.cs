using CSF.TypeReaders;

namespace CSF.Reflection
{
    /// <summary>
    ///     Represents a constructor or method parameter.
    /// </summary>
    public interface IArgument : INameable
    {
        /// <summary>
        ///     Gets the type of the parameter.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Gets the type that is exposed to the runtime directly, potentially being nullable.
        /// </summary>
        public Type ExposedType { get; }

        /// <summary>
        ///     Gets if the parameter is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        ///     Gets if the parameter is optional.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     Gets if the parameter is remainder.
        /// </summary>
        public bool IsRemainder { get; }

        /// <summary>
        ///     Gets the typereader responsible for parsing this type.
        /// </summary>
        public TypeReader TypeReader { get; }
    }
}
