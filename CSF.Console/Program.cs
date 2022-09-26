using CSF;
using CSF.Console;

// Initialize the command framework.
var framework = new CommandStandardizationFramework();

// Register our custom-made guid type reader. A default typereader already exists for Guid, so we replace it. 
framework.RegisterTypeReader(new GuidTypeReader(), true);

// Build the modules found in the current assembly. This will look through the entire csproj the provided type resides in.
var buildResult = await framework.BuildModulesAsync(typeof(Program).Assembly);

if (!buildResult.IsSuccess)
    throw new InvalidOperationException(buildResult.ErrorMessage, buildResult.Exception);

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
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(result.ErrorMessage);
        Console.ResetColor();
    }
}