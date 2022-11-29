using CSF;

//var framework = new CommandFramework(new()
//{
//    DefaultLogLevel = LogLevel.Trace,
//    TypeReaders = new TypeReaderProvider()
//        .Include<LogLevel>(new EnumTypeReader<LogLevel>())
//});

//await framework.BuildModulesAsync(typeof(Program).Assembly);

//while (true)
//{
//    var context = new CommandContext(Console.ReadLine()!);

//    var result = await framework.ExecuteCommandAsync(context);

//    if (!result.IsSuccess)
//        framework.Logger.Write(new Log(LogLevel.Error, result.ErrorMessage, result.Exception));
//}