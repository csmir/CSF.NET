using CSF.Tests.Complex;

namespace CSF.Tests
{
    public class Module : ModuleBase<CommandContext>
    {
        [Command("multiple")]
        public void Test(bool truee, bool falsee)
        {
            Respond($"Success: {truee}, {falsee}");
        }

        [Command("multiple")]
        public void Test(int i1, int i2)
        {
            Respond($"Success: {i1}, {i2}");
        }

        [Command("optional")]
        public void Test(int i = 0, string str = "")
        {
            Respond($"Success: {i}, {str}");
        }

        [Command("nullable")]
        public void Nullable(long? l)
        {
            Respond($"Success: {l}");
        }

        [Command("complex")]
        public void Complex([Complex] ComplexType complex)
        {
            Respond($"({complex.X}, {complex.Y}, {complex.Z}) {complex.Complexer}: {complex.Complexer.X}, {complex.Complexer.Y}, {complex.Complexer.Z}");
        }

        [Command("complexnullable")]
        public void Complex([Complex] ComplexerType? complex)
        {
            Respond($"({complex?.X}, {complex?.Y}, {complex?.Z})");
        }
    }
}
