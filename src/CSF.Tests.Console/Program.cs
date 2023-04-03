using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .AddCommandFramework();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<ICommandFramework>();
var parser = services.GetRequiredService<IParser>();

while (true)
{
    var line = Console.ReadLine()!;

    var parseResult = parser.Parse(line);

    if (!parseResult.IsSuccess)
        continue;

    var context = new CommandContext(parseResult);

    framework.ExecuteAsync(context);
}