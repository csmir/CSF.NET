using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .AddSingleton<TextParser>()
    .AddCommandFramework();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<ICommandFramework>();
var parser = services.GetRequiredService<TextParser>();

while (true)
{
    var input = Console.ReadLine()!;

    var parseResult = parser.Parse(input);

    if (!parseResult.IsSuccess)
        continue;

    var context = new CommandContext(parseResult);

    framework.TryExecuteAsync(context);
}