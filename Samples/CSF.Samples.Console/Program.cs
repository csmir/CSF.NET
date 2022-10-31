using CSF;
using CSF.Console;

// Initialize the command framework.
var framework = new CommandFramework(new()
{
    // Here you can register type readers into the framework.
    TypeReaders = new TypeReaderProvider()
        .Include<Guid>(new GuidTypeReader())
});

// Build the modules found in the current assembly. This will look through the entire csproj the provided type resides in.
await framework.BuildModulesAsync(typeof(Program).Assembly);

// Start a loop to read out the console line and run a command.
while (true)
{
    var commandString = Console.ReadLine();

    if (string.IsNullOrEmpty(commandString))
        continue;

    // Creating this context will automatically parse the parameters provided.
    var context = new CommandContext(commandString);

    // Execute the command.
    var result = await framework.ExecuteCommandAsync(context);

    if (!result.IsSuccess)
        framework.Logger.WriteError("Failed to handle command", result.Exception);
}