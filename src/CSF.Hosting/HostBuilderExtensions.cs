using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

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
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="TService">The <see cref="HostedCommandService{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary hosting necessities.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommands<T, TService>(this IHostBuilder hostBuilder, Action<HostBuilderContext, CommandHostingContext> action)
            where T : CommandConveyor where TService : class, IHostedCommandService
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                var cmdConfig = new CommandHostingContext();

                action.Invoke(context, cmdConfig);

                services.ConfigureCommands<T, TService>(cmdConfig);
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="TService">The <see cref="HostedCommandService{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary hosting necessities.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommands<T, TService>(this IHostBuilder hostBuilder, Action<HostBuilderContext> action)
            where T : CommandConveyor where TService : class, IHostedCommandService
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                action.Invoke(context);

                services.ConfigureCommands<T, TService>(new());
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="TService">The <see cref="HostedCommandService{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommands<T, TService>(this IHostBuilder hostBuilder)
            where T : CommandConveyor where TService : class, IHostedCommandService
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.ConfigureCommands<T, TService>(new());
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandConveyor"/> instance to use.</typeparam>
        /// <typeparam name="TService">The <see cref="HostedCommandService{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="services"></param>
        /// <returns>The same <see cref="IServiceCollection"/> for chaining calls.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection ConfigureCommands<T, TService>(this IServiceCollection services, CommandHostingContext cmdContext)
            where T : CommandConveyor where TService : class, IHostedCommandService
        {
            services.AddSingleton(cmdContext);

            cmdContext.Configuration ??= new();

            services.AddSingleton(cmdContext.Configuration);
            services.AddSingleton<T>();
            services.AddSingleton<CommandFramework<T>>();

            services.AddHostedService<TService>();

            return services;
        }
    }
}