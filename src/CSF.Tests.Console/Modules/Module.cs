using CSF.Tests.Console.Complex;

namespace CSF.Tests.Modules
{
    public class Module : ModuleBase<CommandContext>
    {
        [Command("test")]
        public void Test(bool test, bool test1)
        {
            Success("Success");
        }

        [Command("test")]
        public IResult Test([Complex] ComplexType complex)
        {
            return Success($"{complex.X}{complex.Y}{complex.Z}");
        }
    }
}
