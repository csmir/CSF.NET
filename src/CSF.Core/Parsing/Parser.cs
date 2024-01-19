namespace CSF
{
    public abstract class Parser
    {
        public static Parser Text { get; } = new TextParser();

        public abstract ParserCell Parse(string rawInput);
    }
}
