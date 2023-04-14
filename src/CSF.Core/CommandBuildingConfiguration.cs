using System.Reflection;

namespace CSF
{
    /// <summary>
    ///     Represents a context used to set up the command manager and its child containers.
    /// </summary>
    public sealed class CommandBuildingConfiguration
    {
        /// <summary>
        ///     The assemblies to be used to register
        /// </summary>
        public Assembly[] RegistrationAssemblies { get; set; } = new[] { Assembly.GetEntryAssembly() };

        public IEnumerable<ITypeReader> TypeReaders { get; set; } = TypeReader.CreateDefaultReaders();
    }
}
