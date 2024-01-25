namespace CSF.Parsing
{
    public abstract class Parser<T>
        where T : IEquatable<T>
    {
        public abstract object[] Parse(T value);
    }
}
