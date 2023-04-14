using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSF.Hosting
{
    /// <summary>
    ///     Represents helper method for the <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderHelper
    {
        /// <summary>
        ///     Configures the <see cref="IHostBuilder"/> with a <see cref="HostedCommandManager"/>.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="action">Configuration action for the host and manager.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandManager(this IHostBuilder hostBuilder, Action<HostBuilderContext, CommandBuildingConfiguration> action = null)
        {
            hostBuilder.ConfigureCommandManager<HostedCommandManager>(action);

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the <see cref="IHostBuilder"/> with a customized <see cref="HostedCommandManager"/>.
        /// </summary>
        /// <typeparam name="T">The manager to bind to.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">Configuration action for the host and manager.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandManager<T>(this IHostBuilder hostBuilder, Action<HostBuilderContext, CommandBuildingConfiguration> action = null)
            where T : HostedCommandManager
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var fxContext = new CommandBuildingConfiguration();

                action?.Invoke(hostContext, fxContext);

                services.AddComponents(fxContext);

                services.AddHostedService<T>();
            });

            return hostBuilder;
        }
    }
}
