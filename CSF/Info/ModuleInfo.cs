using CSF.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSF.Info
{
    /// <summary>
    ///     Represents information about the module this command is executed in.
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        ///     The type of this module.
        /// </summary>
        public Type ModuleType { get; }

        /// <summary>
        ///     The constructor used to create an instance of the command type.
        /// </summary>
        public ConstructorInfo Constructor { get; }

        /// <summary>
        ///     The parameters of this module.
        /// </summary>
        public IReadOnlyCollection<ServiceInfo> ServiceTypes { get; }

        /// <summary>
        ///     The range of attributes present on this module.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The range of precondition attributes present on this command.
        /// </summary>
        public IReadOnlyCollection<PreconditionAttribute> Preconditions { get; }

        internal ModuleInfo(Type type)
        {
            IEnumerable<Attribute> GetAttributes()
            {
                foreach (var attr in ModuleType.GetCustomAttributes(true))
                {
                    if (attr is Attribute attribute)
                        yield return attribute;
                }
            }

            IEnumerable<PreconditionAttribute> GetPreconditions()
            {
                foreach (var attr in Attributes)
                {
                    if (attr is PreconditionAttribute precondition)
                        yield return precondition;
                }
            }

            IEnumerable<ServiceInfo> GetServiceTypes()
            {
                foreach (var param in Constructor.GetParameters())
                {
                    yield return new ServiceInfo(param);
                }
            }

            ModuleType = type;
            Constructor = type.GetConstructors()[0];
            ServiceTypes = GetServiceTypes().ToList();
            Attributes = GetAttributes().ToList();
            Preconditions = GetPreconditions().ToList();
        }
    }
}
