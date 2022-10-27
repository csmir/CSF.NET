using System;
using System.Collections.Generic;
using System.Linq;
using SystemParameter = System.Reflection.ParameterInfo;

namespace CSF
{
    /// <summary>
    ///     Represents a single parameter for the method.
    /// </summary>
    public sealed class ParameterInfo
    {
        /// <summary>
        ///     The parameter type.
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        ///     The parameter name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The flags of this parameter.
        /// </summary>
        public ParameterFlags Flags { get; }

        /// <summary>
        ///     The typereader for this parameter.
        /// </summary>
        public ITypeReader Reader { get; }

        /// <summary>
        ///     The attributes for this parameter.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        internal ParameterInfo(SystemParameter paramInfo, ITypeReader reader, bool isNullable)
        {
            Flags.WithNullable(isNullable);
            Flags.WithOptional(paramInfo.IsOptional);

            ParameterType = paramInfo.ParameterType;
            Name = paramInfo.Name;
            Reader = reader;
            Attributes = GetAttributes(paramInfo).ToList();

            if (Attributes.Any(x => x is RemainderAttribute))
            {
                if (ParameterType != typeof(string))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on string parameters.");

                Flags.WithRemainder();
            }
        }

        private IEnumerable<Attribute> GetAttributes(SystemParameter paramInfo)
        {
            foreach (var attribute in paramInfo.GetCustomAttributes(false))
            {
                if (attribute is Attribute attr)
                    yield return attr;
            }
        }
    }
}
