using CSF.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Hosting
{
    public static class HostedCommandManagerResolver
    {
        public static IHostBuilder WithCommands(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
        {
            return builder.WithCommands<HostedCommandManager>(configureDelegate);
        }

        public static IHostBuilder WithCommands<T>(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
            where T : HostedCommandManager
        {
            return builder.WithCommands<T, HostedContextFactory>(configureDelegate);
        }

        public static IHostBuilder WithCommands<TManager, TFactory>(this IHostBuilder builder, [DisallowNull] Action<HostBuilderContext, CommandConfiguration> configureDelegate)
            where TManager : HostedCommandManager where TFactory : class, IContextFactory
        {
            builder.ConfigureServices((context, services) =>
            {
                var config = new CommandConfiguration();

                configureDelegate(context, config);

                services.WithCommands<TManager>(config);

                services.AddScoped<TFactory>();
            });

            return builder;
        }
    }
}
