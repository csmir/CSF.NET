using CSF.Core;
using CSF.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Hosting
{
    public static class HostedCommandManagerResolver
    {
        public static IHostBuilder WithCommands<TFactory>(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
            where TFactory : class, IActionFactory
        {
            return builder.WithCommands<HostedCommandManager, TFactory>(configureDelegate);
        }

        public static IHostBuilder WithCommands<TManager, TFactory>(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
            where TManager : HostedCommandManager where TFactory : class, IActionFactory
        {
            builder.ConfigureServices((context, services) =>
            {
                var config = new CommandConfiguration();
                configureDelegate(context, config);

                services.ModulesAddTransient(config);

                services.TryAddSingleton(config);
                services.TryAddSingleton<TManager>();

                services.AddScoped<TFactory>();
            });

            return builder;
        }
    }
}
