using CSF;

var framework = new CommandFramework(new()
{
    AutoRegisterModules = true,
    RegistrationAssembly = typeof(Program).Assembly
});

while (true)
{
    var context = new CommandContext(Console.ReadLine()!);

    var result = await framework.ExecuteCommandAsync(context);

    if (!result.IsSuccess)
        framework.Logger.Write(new Log(LogLevel.Information, result.ErrorMessage, result.Exception));
}