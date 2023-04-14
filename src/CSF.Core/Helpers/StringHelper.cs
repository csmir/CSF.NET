namespace CSF
{
    public static class StringHelper
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
