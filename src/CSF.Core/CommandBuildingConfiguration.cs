using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents a context used to set up the command manager and its child containers.
    /// </summary>
    public sealed class CommandBuildingConfiguration
    {
        /// <summary>
        ///     The assemblies to be used to register modules and typereaders automatically. Items will be registered when any of the following conditions are true:
        /// </summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item>The type inherits <see cref="TypeReader"/> or <see cref="TypeReader{T}"/>, does not contain undeclared generic parameters and is public.</item>
        ///         <item>The type inherits <see cref="ModuleBase"/> or <see cref="ModuleBase{T}"/>, does not contain undeclared generic parameters and is public.</item>
        ///     </list>
        /// </remarks>
        public Assembly[] RegistrationAssemblies { get; set; } = new[] { Assembly.GetEntryAssembly() };

        /// <summary>
        ///     A range of typereaders that are to be manually registered to all existing readers.
        /// </summary>
        public TypeReader[] TypeReaders { get; set; } = Array.Empty<TypeReader>();

        /// <summary>
        ///     Gets the default configuration that is used when no <see cref="CommandBuildingConfiguration"/> is provided at manager creation. 
        /// </summary>
        public static CommandBuildingConfiguration Default { get; } = new();
    }
}
