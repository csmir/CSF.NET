using System.Diagnostics.CodeAnalysis;

namespace CSF.Tests.Benchmarks
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]

    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]

#pragma warning disable CS0219 // Variable is assigned but its value is never used
    public class BenchmarkModule : ModuleBase<CommandContext>
    {
        [Command("command")]
        public void MyCommand()
        {
            var x = 0;
        }

        [Command("command")]
        public void MyCommand(int i)
        {
            var x = 0;
        }

        [Command("command-op")]
        public void MyOpCommand(int i = 0)
        {
            var x = 0;
        }

        [Group("subcommand")]
        public class InnerModule : ModuleBase<CommandContext>
        {
            [Command("command")]
            public void MyCommand()
            {
                var x = 0;
            }

            [Command("command")]
            public void MyCommand(int i)
            {
                var x = 0;
            }

            [Command("command-op")]
            public void MyOpCommand(int i = 0)
            {
                var x = 0;
            }
        }
    }
#pragma warning restore CS0219 // Variable is assigned but its value is never used
}
