namespace CSF.Hosting
{
    /// <summary>
    ///     Represents an interface that is internally used to resolve the startup assembly. This interface is not intended to be used by developers.
    /// </summary>
    public interface ICommandHostingContext
    {
        /// <summary>
        ///     The configuration used to configure the <see cref="CommandFramework"/>.
        /// </summary>
        public CommandConfiguration Configuration { get; set; }
    }
}
