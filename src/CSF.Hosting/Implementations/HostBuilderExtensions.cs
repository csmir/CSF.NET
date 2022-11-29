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
        /// <typeparam name="T">The <see cref="CommandFramework"/> implementation to use.</typeparam>
        /// <typeparam name="TListener">The <see cref="HostedCommandResolver{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary hosting necessities.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandFramework<T, TListener>(this IHostBuilder hostBuilder, Action<HostBuilderContext, CommandHostingContext> action)
            where T : ImplementationFactory, new() where TListener : class, IHostedCommandResolver
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                var cmdConfig = new CommandHostingContext();

                action.Invoke(context, cmdConfig);

                services.ConfigureCommandFramework<T, TListener>(cmdConfig);
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandFramework"/> implementation to use.</typeparam>
        /// <typeparam name="TListener">The <see cref="HostedCommandResolver{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="action">An action that configures the necessary hosting necessities.</param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandFramework<T, TListener>(this IHostBuilder hostBuilder, Action<HostBuilderContext> action)
            where T : ImplementationFactory, new() where TListener : class, IHostedCommandResolver
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                action.Invoke(context);

                services.ConfigureCommandFramework<T, TListener>(new());
            });

            return hostBuilder;
        }

        /// <summary>
        ///     Configures the necessary settings to set up the command framework for the use of <see cref="Microsoft.Extensions.Hosting"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="CommandFramework"/> implementation to use.</typeparam>
        /// <typeparam name="TListener">The <see cref="HostedCommandResolver{T, TContext}"/> implementation to use.</typeparam>
        /// <param name="hostBuilder"></param>
        /// <returns>The same <see cref="IHostBuilder"/> for chaining calls.</returns>
        public static IHostBuilder ConfigureCommandFramework<T, TListener>(this IHostBuilder hostBuilder)
            where T : ImplementationFactory, new() where TListener : class, IHostedCommandResolver
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.ConfigureCommandFramework<T, TListener>(new());
            });

            return hostBuilder;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceCollection ConfigureCommandFramework<T, TListener>(this IServiceCollection services, CommandHostingContext cmdContext)
            where T : ImplementationFactory, new() where TListener : class, IHostedCommandResolver
        {
            services.AddSingleton(cmdContext);

            cmdContext.Configuration ??= new();

            services.AddSingleton(cmdContext.Configuration);
            services.AddSingleton<T>();
            services.AddSingleton<CommandFramework<T>>();

            services.AddHostedService<TListener>();

            return services;
        }
    }
}