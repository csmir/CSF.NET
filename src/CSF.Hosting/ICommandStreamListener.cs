using Microsoft.Extensions.Hosting;

namespace CSF.Hosting
{
    /// <summary>
    ///     Represents a hosted service that is responsible for receiving and handling command input from less straight forward sources. 
    ///     Implement <see cref="CommandStreamListener{T, TContext}"/> to make use of this behavior.
    /// </summary>
    public interface ICommandStreamListener : IHostedService
    {

    }
}
