namespace CSF.Hosting
{
    /// <summary>
    ///     Represents the context required to configure an <see cref="CommandFramework"/>.
    /// </summary>
    public class CommandHostingContext
    {
        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; set; } = default!;
    }
}
