using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Tests
{
    internal class Modules : ModuleBase<CommandContext>
    {
        [Command("")]
        public IResult Help()
        {
            Console.WriteLine("Success");

            return ExecuteResult.FromSuccess();
        }
    }
}
