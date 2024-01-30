using CSF.Core;
using CSF.Helpers;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

await Host.CreateDefaultBuilder(args)
    .ConfigureCommands<ActionFactory>((context, configuration) =>
    {
        configuration.AsyncApproach = AsyncApproach.Await;

        configuration.TryAddAssembly(Assembly.GetEntryAssembly());
        configuration.ConfigureResultAction(async (context, result, services) =>
        {
            if (result.Success)
            {

            }
            else
            {

            }
        });
    })
    .ConfigureLogging(x =>
    {
        x.AddSimpleConsole();
    })
    .RunConsoleAsync();