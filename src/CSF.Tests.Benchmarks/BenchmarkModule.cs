using System.Diagnostics.CodeAnalysis;

namespace CSF.Tests.Benchmarks
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public class BenchmarkModule : ModuleBase<CommandContext>
    {
        [Command("command")]
        public void MyCommand()
        {

        }

        [Command("command")]
        public void MyCommand(int i)
        {

        }

        [Command("command-op")]
        public void MyOpCommand(int i = 0)
        {

        }

        [Group("subcommand")]
        public class InnerModule : ModuleBase<CommandContext>
        {
            [Command("command")]
            public void MyCommand()
            {
                
            }

            [Command("command")]
            public void MyCommand(int i)
            {

            }

            [Command("command-op")]
            public void MyOpCommand(int i = 0)
            {

            }
        }
    }
}
