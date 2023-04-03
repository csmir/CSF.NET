using Microsoft.Extensions.Hosting;

[assembly: CLSCompliant(true)]
namespace CSF.Hosting
{
    /// <summary>
    ///     Represents a hosted <see cref="CommandFramework{T}"/>. Provides the necessary extensions to set up a hosted environment for CSF.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="HostBuilderExtensions.ConfigureCommands{T, THost}(IHostBuilder)"/> to configure the command pipeline for hosting.
    /// </remarks>
    /// <typeparam name="T">The </typeparam>
    public abstract class HostedCommandService<T> : CommandFramework<T>, IHostedService
        where T : CommandConveyor
    {

    }
}
