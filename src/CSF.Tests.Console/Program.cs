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
        configure.ConfigureDelegate(async (x, y, z) =>
        {
            await Task.CompletedTask;
        });
    })
    .BuildAndRunAsync();