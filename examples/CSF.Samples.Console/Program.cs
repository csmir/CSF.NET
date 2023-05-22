using CSF;
using Microsoft.Extensions.DependencyInjection;

var collection = new ServiceCollection()
    .AddCommandManager();

var services = collection.BuildServiceProvider();

var framework = services.GetRequiredService<CommandManager>();

var input = "helloworld";

var context = new CommandContext(input);

var result = await framework.ExecuteAsync(context);

if (result.Exception != null)
    throw result.Exception;