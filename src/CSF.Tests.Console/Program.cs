using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .WithCommandManager();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

while (true)
{
    var context = new CommandContext(Console.ReadLine()!);

    var result = await framework.ExecuteAsync(context);

    Console.WriteLine(result);
}