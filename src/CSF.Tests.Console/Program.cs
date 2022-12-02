using CSF;

await CommandFramework.CreateDefaultBuilder()
    .ConfigureCommands(configure =>
    {
        configure.TypeReaders
            .Include(new EnumTypeReader<LogLevel>());
        configure.DefaultLogLevel = LogLevel.Trace;
    })
    .Build()
    .RunAsync(resultHandle: (ctx, res) =>
    {
        if (!res.IsSuccess)
            Console.WriteLine(res.ErrorMessage);
    });