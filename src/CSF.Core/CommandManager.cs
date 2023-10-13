[assembly: CLSCompliant(true)]
namespace CSF
{
    /// <summary>
    ///     The root type of the Command Standardization Framework (CSF). This type is responsible for setting up the execution pipeline, handling command input and managing modules.
    /// </summary>
    /// <remarks>
    ///     Guides and documentation can be found at: <see href="https://github.com/csmir/CSF.NET/wiki"/>
    /// </remarks>
    public partial class CommandManager
    {
        private readonly IServiceProvider _services;

        /// <summary>
        ///     Gets the components registered to this manager.
        /// </summary>
        public HashSet<IConditionalComponent> Components { get; }

        /// <summary>
        ///     Creates a new <see cref="CommandManager"/> with default configuration.
        /// </summary>
        /// <param name="services">The services to use to handle command execution and module registration.</param>
        public CommandManager(IServiceProvider services)
            : this(services, CommandBuildingConfiguration.Default)
        {

        }

        /// <summary>
        ///     Creates a new <see cref="CommandManager"/> with provided configuration.
        /// </summary>
        /// <param name="services">The services used to handle command execution and module registration.</param>
        /// <param name="configuration">The configuration that should be used to construct the manager.</param>
        public CommandManager(IServiceProvider services, CommandBuildingConfiguration configuration)
        {
            _services = services;

            Components = configuration.Build();
        }
    }
}
