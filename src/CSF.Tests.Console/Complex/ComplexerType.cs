namespace CSF.Tests.Console.Complex
{
    public class ComplexerType
    {
        public int X, Y, Z;

        [PrimaryConstructor]
        public ComplexerType(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }
    }
}
