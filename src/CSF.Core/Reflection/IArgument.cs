using CSF.TypeReaders;

namespace CSF.Reflection
{

    public interface IArgument : INameable
    {

        public Type Type { get; }


        public Type ExposedType { get; }


        public bool IsNullable { get; }


        public bool IsOptional { get; }


        public bool IsRemainder { get; }


        public TypeConverter TypeReader { get; }
    }
}
