using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureCommandFramework<CommandFramework, CommandResolver>()
    .Build()
    .RunAsync();