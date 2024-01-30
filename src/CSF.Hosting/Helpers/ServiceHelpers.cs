using CSF.Core;
using CSF.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Helpers
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class ServiceHelpers
    {
        public static IHostBuilder ConfigureCommands<TFactory>(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
            where TFactory : class, IActionFactory
        {
            return builder.ConfigureCommands<HostedCommandManager, TFactory>(configureDelegate);
        }

        public static IHostBuilder ConfigureCommands<TManager, TFactory>(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
            where TManager : HostedCommandManager where TFactory : class, IActionFactory
        {
            builder.ConfigureServices((context, services) =>
            {
                var config = new CommandConfiguration();
                configureDelegate(context, config);

                services.AddModules(config);

                services.TryAddSingleton(config);
                services.TryAddSingleton<TManager>();

                services.AddScoped<TFactory>();
            });

            return builder;
        }
    }
}
