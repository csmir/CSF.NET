using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .WithCommandManager();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

while (true)
{
    var input = Console.ReadLine()!;

    var context = new CommandContext(input);

    var result = await framework.ExecuteAsync(context);

    if (result.Failed())
        Console.WriteLine(result);
}