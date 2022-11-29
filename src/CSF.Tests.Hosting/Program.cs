using CSF;
using CSF.Hosting;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureCommandFramework<ImplementationFactory, CommandResolver>()
    .Build()
    .RunAsync();