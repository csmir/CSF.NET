using Microsoft.Extensions.Hosting;

namespace CSF.Hosting
{
    /// <summary>
    ///     Represents a hosted service that is responsible for receiving and handling command input from less straight forward sources. 
    ///     Implement <see cref="HostedCommandResolver{T, TContext}"/> to make use of this behavior.
    /// </summary>
    public interface IHostedCommandResolver : IHostedService
    {
        /// <summary>
        ///     Represents the serviceprovider of the <see cref="IHost"/> executing this service.
        /// </summary>
        public IServiceProvider Services { get; }
    }
}
