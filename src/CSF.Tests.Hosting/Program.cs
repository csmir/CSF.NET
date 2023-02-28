using CSF;
using CSF.Hosting;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureCommands<CommandConveyor, CommandService>((hostContext, commandConfig) =>
    {
        commandConfig.DefaultLogLevel = LogLevel.Trace;
        commandConfig.Prefixes
            .Include(new StringPrefix(hostContext.Configuration["Prefixes:DefaultPrefix"]));
    })
    .Build()
    .RunAsync();