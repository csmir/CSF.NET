using CSF.TypeReaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        ///     The typereader for this parameter.
        /// </summary>
        public ITypeReader Reader { get; }

        /// <summary>
        ///     The attributes for this parameter.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        internal ParameterInfo(System.Reflection.ParameterInfo info, ITypeReader reader)
        {
            IEnumerable<Attribute> GetAttributes()
            {
                foreach (var attribute in info.GetCustomAttributes(false))
                {
                    if (attribute is Attribute attr)
                        yield return attr;
                }
            }

            IsOptional = info.IsOptional;
            Type = info.ParameterType;
            Reader = reader;
            Attributes = GetAttributes().ToList();
        }
    }
}
