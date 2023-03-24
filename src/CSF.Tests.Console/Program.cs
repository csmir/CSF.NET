using CSF;

await CommandFramework.CreateDefaultBuilder()
    .ConfigureCommands(configure =>
    {
        configure.TypeReaders
            .Include(new EnumTypeReader<LogLevel>());
        configure.DefaultLogLevel = LogLevel.Trace;
        configure.Parser.Prefixes
            .Include(new StringPrefix("!"));
    })
    .BuildAndRunAsync();