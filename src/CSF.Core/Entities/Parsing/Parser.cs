namespace CSF
{
    public abstract class Parser
    {
        public static Parser Text { get; } = new TextParser();

        public abstract ParserCell Parse(string rawInput);
    }

    public static class ParserHelper
    {
        public static bool HasStringPrefix(this string rawInput, string prefix)
        {
            Assert.IsNotWhitespace(rawInput);

            return rawInput.StartsWith(prefix);
        }

        public static bool HasCharPrefix(this string rawInput, char prefix)
        {
            Assert.IsNotWhitespace(rawInput);

            return rawInput[0] == prefix;
        }
    }
}
