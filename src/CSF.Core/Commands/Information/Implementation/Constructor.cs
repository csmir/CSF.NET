﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents information about the primary constructor of a module.
    /// </summary>
    public class Constructor : IComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        //// <inheritdoc/>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        ///     The parameters of this module.
        /// </summary>
        public IReadOnlyCollection<DependencyParameter> Dependencies { get; }

        /// <summary>
        ///     The constructor entry point.
        /// </summary>
        public System.Reflection.ConstructorInfo EntryPoint { get; }

        internal Constructor(Type type)
        {
            EntryPoint = GetEntryConstructor(type);
            Attributes = GetAttributes(EntryPoint).ToList();

            Dependencies = GetDependencies(EntryPoint).ToList();

            Name = EntryPoint.Name;
        }

        /// <summary>
        ///     Builds the constructor from the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="provider">The services to build this constructor with.</param>
        /// <returns>An object representing the built object instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Construct(IServiceProvider provider)
        {
            var services = new List<object>();
            foreach (var dependency in Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return null;

                    services.Add(t);
                }
            }

            var obj = EntryPoint.Invoke(services.ToArray());

            return obj;
        }

        /// <summary>
        ///     Builds the constructor from the provided parameters.
        /// </summary>
        /// <param name="parameters">The parameters to build this object from.</param>
        /// <returns>An object representing the built object instance.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Construct(List<object> parameters)
        {
            var obj = EntryPoint.Invoke(parameters.ToArray());

            return obj;
        }

        private System.Reflection.ConstructorInfo GetEntryConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            if (!constructors.Any())
                throw new InvalidOperationException($"Found no constructor on provided module type: {type.Name}");

            var constructor = constructors[0];

            if (constructors.Length is 1)
                return constructor;

            for (int i = 0; i < constructors.Length; i++)
                foreach (var attribute in constructors[i].GetCustomAttributes(true))
                    if (attribute is PrimaryConstructorAttribute)
                        return constructors[i];

            return constructor;
        }

        private IEnumerable<Attribute> GetAttributes(System.Reflection.ConstructorInfo ctorInfo)
        {
            foreach (var obj in ctorInfo.GetCustomAttributes(false))
                if (obj is Attribute attribute)
                    yield return attribute;
        }

        private IEnumerable<DependencyParameter> GetDependencies(System.Reflection.ConstructorInfo ctorInfo)
        {
            foreach (var param in ctorInfo.GetParameters())
                yield return new DependencyParameter(param);
        }
    }
}