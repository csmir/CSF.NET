using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSF.Hosting
{
    /// <summary>
    ///     Represents the necessary extensions to set up a hosted environment for CSF.NET.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandFramework"/> implementation to use.</typeparam>
        /// <typeparam name="TConfig">The <see cref="CommandConfiguration"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary settings of the framework.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandFramework<T, TConfig>(this IHostBuilder hostBuilder, Action<HostBuilderContext, CommandHostingContext<TConfig>> action)
            where T : CommandFramework
            where TConfig : CommandConfiguration
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                var config = new CommandHostingContext<TConfig>();

                action(context, config);

                services.AddSingleton<ICommandHostingContext, CommandHostingContext<TConfig>>(x => config);
                services.AddSingleton(config.Configuration);
                services.AddSingleton<T>();
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the <see cref="ICommandStreamListener"/> responsible for resolving command input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hostBuilder"></param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandStream<T>(this IHostBuilder hostBuilder)
            where T : class, ICommandStreamListener
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<T>();
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Registers all modules in the assembly configured at <see cref="ConfigureCommandFramework{T, TConfig}(IHostBuilder, Action{HostBuilderContext, CommandHostingContext{TConfig}})"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <returns>The same <see cref="IHost"/> for chaining calls.</returns>
        public static IHost RegisterModules<T>(this IHost host)
            where T : CommandFramework
        {
            var configuration = host.Services.GetRequiredService<ICommandHostingContext>();
            var framework = host.Services.GetRequiredService<T>();

            framework.BuildModulesAsync(configuration.RegistrationAssembly).GetAwaiter().GetResult();

            return host;
        }
    }
}