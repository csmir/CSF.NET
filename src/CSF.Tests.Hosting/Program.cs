using CSF;
using CSF.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .WithCommandFramework()
    .RunConsoleAsync();