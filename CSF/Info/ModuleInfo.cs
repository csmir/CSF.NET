using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSF.Info
{
    public class ModuleInfo
    {
        /// <summary>
        ///     The constructor used to create an instance of the command type.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        ///     The parameters of this module.
        /// </summary>
        public IEnumerable<ParameterInfo> Parameters { get; }

        /// <summary>
        ///     The range of attributes present on this module.
        /// </summary>
        public IEnumerable<Attribute> Attributes { get; }

        internal ModuleInfo(object[] attributes, ConstructorInfo ctor)
        {
            IEnumerable<Attribute> GetAttributes()
            {
                foreach (var attr in attributes)
                {
                    if (attr is Attribute attribute)
                        yield return attribute;
                }
            }

            IEnumerable<ParameterInfo> GetParameters()
            {
                foreach (var param in ctor.GetParameters())
                {
                    yield return new ParameterInfo(param);
                }
            }

            Constructor = ctor;
            Parameters = GetParameters();
            Attributes = GetAttributes();
        }
    }
}
