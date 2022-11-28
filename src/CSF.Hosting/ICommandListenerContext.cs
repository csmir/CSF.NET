namespace CSF.Hosting
{
    /// <summary>
    ///     Represents the interface that implements non generic members of <see cref="HostedCommandResolver{T, TContext}"/>.
    /// </summary>
    public interface ICommandListenerContext
    {
        /// <summary>
        ///     The source to cancel all tokens and quit out of <see cref="HostedCommandResolver{T, TContext}.GetInputStreamAsync(CancellationToken)"/>.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
