using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Tests.Benchmarks
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public class Program
    {
        private readonly static IServiceProvider _services;
        private readonly static CommandManager _manager;

        static Program()
        {
            _services = new ServiceCollection()
            .AddCommandManager(x =>
            {
                x.RegistrationAssemblies = new[] { typeof(Program).Assembly };
            })
            .BuildServiceProvider();

            _manager = _services.GetRequiredService<CommandManager>();

            _noparam = new("command");
            _param = new("command 1");
            _paramnoprov = new("command-op");
            _noparamnested = new("subcommand command");
            _paramnested = new("subcommand command 1");
        }

        //private static async Task Main()
        //{
        //    var pg = new Program();

        //    await pg.Parametered();
        //    await pg.Parameterless();
        //    await pg.NestedParametered();
        //    await pg.NestedParameterless();
        //    await pg.ParameteredUnprovided();
        //}

        private static void Main()
           => BenchmarkRunner.Run<Program>();

        private readonly static CommandContext _noparam;
        [Benchmark]
        public async Task Parameterless()
            => await _manager.ExecuteAsync(_noparam);

        private readonly static CommandContext _param;
        [Benchmark]
        public async Task Parametered()
            => await _manager.ExecuteAsync(_param);

        private readonly static CommandContext _paramnoprov;
        [Benchmark]
        public async Task ParameteredUnprovided()
            => await _manager.ExecuteAsync(_paramnoprov);

        private readonly static CommandContext _noparamnested;
        [Benchmark]
        public async Task NestedParameterless()
            => await _manager.ExecuteAsync(_noparamnested);

        private readonly static CommandContext _paramnested;
        [Benchmark]
        public async Task NestedParametered()
            => await _manager.ExecuteAsync(_paramnested);
    }
}