[assembly: CLSCompliant(true)]
namespace CSF
{
    /// <summary>
    ///     The root class of CSF, responsible for managing commands and their execution. 
    /// </summary>
    public partial class CommandManager
    {
        private readonly IServiceProvider _services;

        public IConditionalComponent[] Components { get; }

        public CommandManager(IServiceProvider services)
            : this(services, CommandBuildingConfiguration.Default)
        {

        }

        public CommandManager(IServiceProvider services, CommandBuildingConfiguration configuration)
        {
            _services = services;
            Components = configuration.Build();
        }
    }
}
