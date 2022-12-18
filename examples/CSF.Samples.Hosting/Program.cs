using CSF;
using CSF.Hosting;
using CSF.Samples.Hosting;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder();

host.ConfigureCommandFramework<CommandFramework, CommandResolver>((host, commands) =>
{
    commands.Configuration = new()
    {

    };
});

await host.RunConsoleAsync();