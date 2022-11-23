using CSF;
using CSF.Hosting;
using CSF.Tests.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureCommandFramework<CommandFramework, CommandConfiguration>((ctx, cmdctx) =>
{
    cmdctx.Configuration = new()
    {
        DefaultLogLevel = LogLevel.Debug,
    };
});

builder.ConfigureCommandStream<MyStreamListener>();

var app = builder.Build();

app.RegisterModules<CommandFramework>();