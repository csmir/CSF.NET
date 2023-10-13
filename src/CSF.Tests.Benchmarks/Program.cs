using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace CSF.Tests.Benchmarks
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]

    [MemoryDiagnoser]
    public class Program
    {
        private readonly static IServiceProvider _services;
        private readonly static CommandManager _manager;

        static Program()
        {
            _services = new ServiceCollection()
            .WithCommandManager(x =>
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

        private static void Main()
           => BenchmarkRunner.Run<Program>();

        private readonly static CommandContext _noparam;
        [Benchmark]
        public void Parameterless()
            => _manager.ExecuteAsync(_noparam);

        private readonly static CommandContext _param;
        [Benchmark]
        public void Parametered()
            => _manager.ExecuteAsync(_param);

        private readonly static CommandContext _paramnoprov;
        [Benchmark]
        public void ParameteredUnprovided()
            => _manager.ExecuteAsync(_paramnoprov);

        private readonly static CommandContext _noparamnested;
        [Benchmark]
        public void NestedParameterless()
            => _manager.ExecuteAsync(_noparamnested);

        private readonly static CommandContext _paramnested;
        [Benchmark]
        public void NestedParametered()
            => _manager.ExecuteAsync(_paramnested);
    }
}