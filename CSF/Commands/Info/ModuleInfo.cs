﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSF
{
    /// <summary>
    ///     Represents information about the module this command is executed in.
    /// </summary>
    public sealed class ModuleInfo
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
            ModuleType = type;
            Constructor = type.GetConstructors()[0];
            ServiceTypes = GetServiceTypes(Constructor).ToList();
            Attributes = GetAttributes(ModuleType).ToList();
            Preconditions = GetPreconditions(Attributes).ToList();
        }

        private IEnumerable<Attribute> GetAttributes(Type type)
        {
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (attr is Attribute attribute)
                    yield return attribute;
            }
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions(IReadOnlyCollection<Attribute> attributes)
        {
            foreach (var attr in attributes)
            {
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
            }
        }

        private IEnumerable<ServiceInfo> GetServiceTypes(ConstructorInfo ctor)
        {
            foreach (var param in ctor.GetParameters())
            {
                var type = param.ParameterType;
                var nullableType = Nullable.GetUnderlyingType(type);
                var isNullable = !(nullableType is null);

                if (isNullable)
                    type = nullableType;

                yield return new ServiceInfo(type, param.IsOptional, isNullable);
            }
        }
    }
}
