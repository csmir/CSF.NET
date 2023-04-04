using System.Diagnostics.CodeAnalysis;

namespace CSF
{
    public interface IParser
    {
        public bool TryParse(object rawInput, [NotNullWhen(true)] out ParserOutput result);
    }
}
