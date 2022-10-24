using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Tests
{
    internal class Modules : ModuleBase<CommandContext>
    {
        [Command("help")]
#pragma warning disable CA1822 // Mark members as static
        public IResult Help()
#pragma warning restore CA1822 // Mark members as static
        {
            Console.WriteLine("Success");

            return ExecuteResult.FromSuccess();
        }
    }
}
