using CSF.Hosting;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

await Host.CreateDefaultBuilder(args)
    .WithCommands<Factory>((context, config) =>
    {
        config.Assemblies = [ Assembly.GetEntryAssembly() ];
    })
    .ConfigureLogging(x =>
    {
        x.AddSimpleConsole();
    })
    .RunConsoleAsync();