using System.Reflection;

namespace CSF.Hosting
{
    /// <summary>
    ///     Represents an interface that is internally used to resolve the startup assembly. This interface is not intended to be used by developers.
    /// </summary>
    public interface ICommandHostingContext
    {
        /// <summary>
        ///     The assembly used to register modules in the project.
        /// </summary>
        public Assembly RegistrationAssembly { get; set; }
    }
}
