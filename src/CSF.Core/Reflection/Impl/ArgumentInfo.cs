using CSF.Helpers;
using CSF.TypeReaders;
using System.Reflection;

namespace CSF.Reflection
{

    public sealed class ArgumentInfo : IArgument
    {
        public string Name { get; }

        public Type Type { get; }

        public Type ExposedType { get; }

        public bool IsNullable { get; }

        public bool IsOptional { get; }

        public bool IsRemainder { get; }

        public Attribute[] Attributes { get; }

        public TypeReader TypeReader { get; }

        internal ArgumentInfo(ParameterInfo parameterInfo, IDictionary<Type, TypeReader> typeReaders)
        {
            var underlying = Nullable.GetUnderlyingType(parameterInfo.ParameterType);
            var attributes = parameterInfo.GetAttributes(false);

            if (underlying != null)
            {
                IsNullable = true;
                Type = underlying;
            }
            else
            {
                IsNullable = false;
                Type = parameterInfo.ParameterType;
            }

            if (parameterInfo.IsOptional)
                IsOptional = true;
            else
                IsOptional = false;

            if (attributes.Contains<RemainderAttribute>(false))
                IsRemainder = true;
            else
                IsRemainder = false;

            if (Type.IsEnum)
                TypeReader = EnumTypeReader.GetOrCreate(Type);

            else if (Type != typeof(string) && Type != typeof(object))
                TypeReader = typeReaders[Type];

            Attributes = attributes;
            ExposedType = parameterInfo.ParameterType;
            Name = parameterInfo.Name;
        }

        public override string ToString()
            => $"{Type.Name} {Name}";
    }
}
