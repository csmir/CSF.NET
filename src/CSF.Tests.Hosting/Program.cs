using CSF.Helpers;
using CSF.Hosting;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

await Host.CreateDefaultBuilder(args)
    .WithCommands<Factory>((context, configuration) =>
    {
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