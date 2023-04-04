using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .AddCommandManager()
    .AddTypeReader<EnumTypeReader<ServiceLifetime>>();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();
var parser = new TextParser();

while (true)
{
    var input = Console.ReadLine()!;

    if (parser.TryParse(input, out var output))
    {
        var context = new CommandContext(output);

        var result = await framework.TryExecuteAsync(context);

        if (!result.IsSuccess)
            Console.WriteLine(result.ErrorMessage);
    }
}