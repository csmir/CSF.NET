using CSF.Core;
using CSF.Helpers;
using CSF.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

await Host.CreateDefaultBuilder(args)
    .ConfigureCommands((context, configuration) =>
    {
        configuration.AsyncApproach = AsyncApproach.Await;

        configuration.TryAddAssembly(Assembly.GetEntryAssembly());
        configuration.ConfigureResultAction((context, result, services) =>
        {
            if (result.Success)
            {
                return Task.CompletedTask;
            }
            else
            {
                return Task.CompletedTask;
            }
        });
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<CommandHandler>();
    })
    .ConfigureLogging(x =>
    {
        x.AddSimpleConsole();
    })
    .RunConsoleAsync();