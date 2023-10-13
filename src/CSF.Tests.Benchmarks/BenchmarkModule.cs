using System.Diagnostics.CodeAnalysis;

namespace CSF.Tests.Benchmarks
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public class BenchmarkModule : ModuleBase<CommandContext>
    {
        [Command("command")]
        public Task MyCommand()
        {
            return Task.CompletedTask;
        }

        [Command("command")]
        public Task MyCommand(int i)
        {
            return Task.CompletedTask;
        }

        [Command("command-op")]
        public Task MyOpCommand(int i = 0)
        {
            return Task.CompletedTask;
        }

        [Group("subcommand")]
        public class InnerModule : ModuleBase<CommandContext>
        {
            [Command("command")]
            public Task MyCommand()
            {
                return Task.CompletedTask;
            }

            [Command("command")]
            public Task MyCommand(int i)
            {
                return Task.CompletedTask;
            }

            [Command("command-op")]
            public Task MyOpCommand(int i = 0)
            {
                return Task.CompletedTask;
            }
        }
    }
}
