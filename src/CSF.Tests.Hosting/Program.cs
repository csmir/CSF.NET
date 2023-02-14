using CSF;
using CSF.Hosting;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;

#pragma warning disable CS8321

await Host.CreateDefaultBuilder(args)
    .ConfigureCommands<CommandConveyor, CommandService>((hostContext, commandConfig) =>
    {
        commandConfig.DefaultLogLevel = LogLevel.Trace;
        commandConfig.Prefixes
            .Include(new StringPrefix(hostContext.Configuration["Prefixes:DefaultPrefix"]));
        commandConfig.UseTopLevelCommands = true;
    })
    .Build()
    .RunAsync();

// tlc 5 true
[Command("top-level-command", "tlc")]
static IResult MyCommand(int i, bool b)
{
    return ExecuteResult.FromSuccess();
}

// atlc 22 true
[Command("another-top-level-command", "atlc")]
static IResult AnotherCommand(int i, bool b)
{
    return MyCommand(i, b);
}