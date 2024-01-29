using CSF.Core;
using CSF.Parsing;
using CSF.Helpers;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .WithCommands(_ => { });

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

var parser = new StringParser();

while (true)
{
    var input = parser.Parse(Console.ReadLine()!);

    await framework.ExecuteAsync(null, input);
}