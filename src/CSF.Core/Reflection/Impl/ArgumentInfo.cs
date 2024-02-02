﻿using CSF.Core;
using CSF.Helpers;
using CSF.TypeConverters;
using System.Reflection;

namespace CSF.Reflection
{
    /// <inheritdoc />
    public sealed class ArgumentInfo : IArgument
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public Type ExposedType { get; }

        /// <inheritdoc />
        public bool IsNullable { get; }

        /// <inheritdoc />
        public bool IsOptional { get; }

        /// <inheritdoc />
        public bool IsRemainder { get; }

        /// <inheritdoc />
        public Attribute[] Attributes { get; }

        /// <inheritdoc />
        public TypeConverter Converter { get; }

        internal ArgumentInfo(ParameterInfo parameterInfo, IDictionary<Type, TypeConverter> typeReaders)
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
                Converter = EnumTypeReader.GetOrCreate(Type);

            else if (Type != typeof(string) && Type != typeof(object))
                Converter = typeReaders[Type];

            Attributes = attributes;
            ExposedType = parameterInfo.ParameterType;
            Name = parameterInfo.Name;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{Type.Name} {Name}";
    }
}
