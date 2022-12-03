using CSF;

await CommandFramework.CreateDefaultBuilder()
    .ConfigureCommands(configure =>
    {
        configure.TypeReaders
            .Include(new EnumTypeReader<LogLevel>());
        configure.DefaultLogLevel = LogLevel.Trace;
    })
    .ConfigureHandlers(configure =>
    {
        configure.ConfigureDelegate(async (ctx, result, ctoken) =>
        {
            await Task.CompletedTask;
        });
    })
    .BuildAndRunAsync();