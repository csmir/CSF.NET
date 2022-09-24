using CSF.TypeReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemParameter = System.Reflection.ParameterInfo;

namespace CSF.Info
{
    /// <summary>
    ///     Represents a single parameter for the method.
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        ///     The parameter type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Defines if the parameter is optional.
        /// </summary>
        public bool IsOptional { get; }

        /// <summary>
        ///     Defines the parameter is nullable.
        /// </summary>
        public bool IsNullable { get; }

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
            IsNullable = isNullable;
            IsOptional = paramInfo.IsOptional;
            Type = paramInfo.ParameterType;
            Reader = reader;
            Attributes = GetAttributes(paramInfo).ToList();
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
