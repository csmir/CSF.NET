using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .WithCommandManager();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

while (true)
{
    var context = new CommandContext(Console.ReadLine()!);

    var result = framework.ExecuteAsync(context);

    if (result.Failed())
        Console.WriteLine(result);
}