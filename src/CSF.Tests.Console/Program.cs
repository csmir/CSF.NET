﻿using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .AddSingleton<TextParser>()
    .AddCommandManager();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();
var parser = services.GetRequiredService<TextParser>();

while (true)
{
    var input = Console.ReadLine()!;

    if (parser.TryParse(input, out var output))
    {
        var context = new CommandContext(output);

        await framework.TryExecuteAsync(context);
    }
}