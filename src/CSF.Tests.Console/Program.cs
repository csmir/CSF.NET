﻿using CSF.Core;
using CSF.Helpers;
using CSF.Parsing;
using CSF.Tests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var collection = new ServiceCollection()
    .ConfigureCommands(configuration =>
    {
        configuration.AsyncApproach = AsyncApproach.Await;
        configuration.ConfigureResultAction(async (context, result, services) =>
        {
            if (result.Success)
            {
                await Task.CompletedTask;
            }
            else
            {
                Console.WriteLine(result.Exception);
            }
        });
        configuration.WithAssemblies(Assembly.GetEntryAssembly());
    });

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

var parser = new StringParser();

while (true)
{
    var input = parser.Parse(Console.ReadLine()!);

    await framework.TryExecuteAsync(new LoggingContext(), input);
}