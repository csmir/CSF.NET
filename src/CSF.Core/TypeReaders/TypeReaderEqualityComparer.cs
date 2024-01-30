using System.Diagnostics.CodeAnalysis;

namespace CSF.TypeReaders
{
    public sealed class TypeReaderEqualityComparer : IEqualityComparer<TypeReader>
    {
        private static readonly TypeReaderEqualityComparer _i = new();

        public bool Equals(TypeReader x, TypeReader y)
        {
            if (x == y)
                return true;

            if (x.Type == y.Type)
                return true;

            return false;
        }

        public int GetHashCode([DisallowNull] TypeReader obj)
        {
            return obj.GetHashCode();
        }

        public static TypeReaderEqualityComparer Default
        {
            get
            {
                return _i;
            }
        }
    }
}
