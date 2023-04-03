using System;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    /// <summary>
    ///     Represents information about the module this command is executed in.
    /// </summary>
    public sealed class Module : IConditionalComponent
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string[] Aliases { get; }

        /// <inheritdoc/>
        public IList<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IList<IPrecondition> Preconditions { get; }

        /// <summary>
        ///     The components of this module.
        /// </summary>
        public IList<IConditionalComponent> Components { get; }

        /// <summary>
        ///     The type of this module.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     The root module. <see langword="null"/> if not available.
        /// </summary>
        public Module Root { get; }

        public Module(TypeReaderContainer typeReaders, Type type, Module rootModule = null, string expectedName = null, string[] aliases = null)
        {
            if (rootModule != null)
                Root = rootModule;

            Type = type;

            Attributes = (Root?.Attributes.Concat(GetAttributes()) ?? GetAttributes())
                .ToList();
            Preconditions = (Root?.Preconditions.Concat(GetPreconditions()) ?? GetPreconditions())
                .ToList();

            Name = expectedName ?? type.Name;
            Aliases = aliases ?? new string[] { Name };

            Components = GetComponents(typeReaders).ToList();
        }

        private IEnumerable<IConditionalComponent> GetComponents(TypeReaderContainer typeReaders)
        {
            foreach (var method in Type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(true);

                string[] aliases = Array.Empty<string>();
                foreach (var attribute in attributes)
                {
                    if (attribute is CommandAttribute commandAttribute)
                    {
                        aliases = commandAttribute.Aliases;
                    }
                }

                if (!aliases.Any())
                    continue;

                yield return new Command(typeReaders, this, method, aliases);
            }

            foreach (var group in Type.GetNestedTypes())
            {
                foreach (var attribute in group.GetCustomAttributes(true))
                {
                    if (attribute is GroupAttribute gattribute)
                        yield return new Module(typeReaders, group, this, gattribute.Name, gattribute.Aliases);
                }
            }
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attr in Type.GetCustomAttributes(true))
                if (attr is Attribute attribute)
                    yield return attribute;
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions()
        {
            foreach (var attr in Attributes)
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
        }

        public override string ToString()
            => $"{(Root != null ? $"{Root}." : "")}{(Type.Name != Name ? $"{Type.Name}['{Name}']" : $"{Name}")}";
    }
}
