using CSF;
using CSF.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureCommands<CommandConveyor, CommandService>((hostContext, commandConfig) =>
    {

    })
    .RunConsoleAsync();