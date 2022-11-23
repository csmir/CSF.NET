namespace CSF.Hosting
{
    /// <summary>
    ///     Represents the context required to configure an <see cref="CommandFramework"/>.
    /// </summary>
    public class CommandHostingContext : ICommandHostingContext
    {
        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; set; } = default!;

        /// <inheritdoc/>
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
