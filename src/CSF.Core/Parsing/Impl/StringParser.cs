using System.Text;

namespace CSF.Parsing
{
    public class StringParser : Parser<string>
    {
        const char quote = '"';
        const char whitespace = ' ';

        public override object[] Parse(string toParse)
        {
            var arr = Array.Empty<string>();
            var sb = new StringBuilder(0, toParse.Length);
            var quoted = false;

            // Adds SB content to array & resets
            void SAddReset()
            {
                // if anything exists, otherwise skip
                if (sb.Length > 0)
                {
                    var size = arr.Length;
                    Array.Resize(ref arr, size + 1);

                    arr[size] = sb.ToString();

                    // clear for next range
                    sb.Clear();
                }
            }

            // enter loop for string inner char[]
            for (int i = 0; i < toParse.Length; i++)
            {
                // if startquote found, skip space check & continue until next occurrence of quote
                if (quoted)
                {
                    // next quote occurrence
                    if (toParse[i] is quote)
                    {
                        // add discovered until now, skipping quote itself
                        SAddReset();

                        // set quoted to false, quoted range is handled
                        quoted = false;

                        // next loop step
                        continue;
                    }

                    // add char in quote range
                    sb.Append(toParse[i]);

                    // dont allow the checks below this statement, next loop step
                    continue;
                }

                // check for startquote
                if (toParse[i] is quote)
                {
                    // check end of loop, skipping add
                    if (i + 1 == toParse.Length)
                    {
                        break;
                    }

                    // add all before quote
                    SAddReset();

                    // set startquote discovery to true
                    quoted = true;

                    continue;
                }

                // check for whitespace
                if (toParse[i] is whitespace)
                {
                    // add all before whitespace, skip whitespace itself
                    SAddReset();

                    continue;
                }

                // nomatch for above, add character to current range
                sb.Append(toParse[i]);
            }

            // if loop ended, do final add
            SAddReset();

            return arr;
        }
    }
}
