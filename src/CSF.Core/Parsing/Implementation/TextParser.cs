namespace CSF
{
    /// <summary>
    ///     Represents a parser for text command input.
    /// </summary>
    public class TextParser : Parser
    {
        public override ParserCell Parse(string rawInput)
        {
            var splitInput = rawInput.Split(' ');

            var hasName = false;

            var param = new List<string>();
            var partial = new List<string>();

            var paramName = "";
            var namedParam = new Dictionary<string, string>();

            foreach (var part in splitInput)
            {
                if (!hasName)
                {
                    param.Add(part);
                    continue;
                }

                if (partial.Any())
                {
                    if (part.EndsWith("\""))
                    {
                        partial.Add(part.Replace("\"", ""));

                        if (paramName is "")
                            param.Add(string.Join(" ", partial));
                        else
                        {
                            namedParam.Add(paramName, string.Join(" ", partial));
                            paramName = "";
                        }

                        partial.Clear();
                        continue;
                    }
                    partial.Add(part);
                    continue;
                }

                if (part.StartsWith('"'))
                {
                    if (part.EndsWith('"'))
                    {
                        if (paramName is "")
                            param.Add(part.Replace("\"", ""));
                        else
                        {
                            namedParam.Add(paramName, part.Replace("\"", ""));
                            paramName = "";
                        }
                    }
                    else
                        partial.Add(part.Replace("\"", ""));
                    continue;
                }

                if (part.StartsWith("-"))
                    foreach (var c in part[1..])
                        namedParam.Add(c.ToString(), null);

                if (part.StartsWith("--"))
                {
                    if (!part.EndsWith(":"))
                        namedParam.Add(part[1..], null!);
                    else
                        paramName = part[1..^1];
                    continue;
                }

                param.Add(part);
            }

            return new(param.ToArray(), namedParam);
        }
    }
}
