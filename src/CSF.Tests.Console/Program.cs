//using CSF;
//using Microsoft.Extensions.DependencyInjection;

//var collection = new ServiceCollection()
//    .WithCommandManager();

//var services = collection.BuildServiceProvider();

//var framework = services.GetRequiredService<CommandManager>();

//var delayed = new CommandContext("delayed");
//var direct = new CommandContext("direct");

//framework.ExecuteAsync(delayed);
//framework.ExecuteAsync(direct);

//await Task.Delay(Timeout.Infinite);

//while (true)
//{
//    var context = new CommandContext(Console.ReadLine()!);

//    var result = framework.ExecuteAsync(context);

//    if (result.Failed(out var failure))
//        Console.WriteLine(failure.Exception);
//}

using CSF.Parsing;

var parser = new StringParser();

while (true)
{
    var text = Console.ReadLine();

    var value = parser.Parse(text);

    foreach (var item in value)
        Console.WriteLine("-> " + item);
}