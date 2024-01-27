using CSF.Helpers;
using CSF.TypeReaders;
using System.Reflection;

namespace CSF.Reflection
{

    public class ComplexArgumentInfo : IArgument, IArgumentBucket
    {

        public string Name { get; }


        public Type Type { get; }


        public Type ExposedType { get; }


        public bool IsNullable { get; }


        public bool IsOptional { get; }


        public bool IsRemainder { get; }


        public Attribute[] Attributes { get; }


        public IArgument[] Parameters { get; }


        public bool HasParameters { get; }


        public int MinLength { get; }


        public int MaxLength { get; }


        public TypeReader TypeReader { get; }


        public ConstructorInfo Constructor { get; }

        internal ComplexArgumentInfo(ParameterInfo parameterInfo, IDictionary<Type, TypeReader> typeReaders)
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

            var constructor = Type.GetConstructors()[0];
            var parameters = constructor.GetParameters(typeReaders);

            if (parameters.Length > 0)
            {
                ThrowHelpers.InvalidOp("Complex types are expected to have at least 1 constructor parameter.");
            }

            var (minLength, maxLength) = parameters.GetLength();

            IsRemainder = false;
            TypeReader = null;

            MinLength = minLength;
            MaxLength = maxLength;

            Constructor = constructor;
            Parameters = parameters;
            HasParameters = parameters.Length > 0;

            Attributes = attributes;

            ExposedType = parameterInfo.ParameterType;
            Name = parameterInfo.Name;
        }


        public override string ToString()
            => $"{Type.Name} ({string.Join<IArgument>(", ", Parameters)}) {Name}";
    }
}
