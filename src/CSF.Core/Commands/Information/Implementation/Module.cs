using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public Module(Type type, Module rootModule = null, string expectedName = null, string[] aliases = null)
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

            Components = GetComponents()
                .ToList();
        }

        public async ValueTask<SearchResult> SearchAsync<T>(T context, CancellationToken cancellationToken)
            where T : IContext
        {
            var matches = Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.CastWhere<Command>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.CastWhere<Module>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return await groups[0].SearchAsync(context, cancellationToken);
                }

                else
                    return SearchResult.FromError("No command found!");
            }

            return SearchResult.FromSuccess(commands);
        }

        private IEnumerable<IConditionalComponent> GetComponents()
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

                yield return new Command(this, method, aliases);
            }

            foreach (var group in Type.GetNestedTypes())
            {
                foreach (var attribute in group.GetCustomAttributes(true))
                {
                    if (attribute is GroupAttribute gattribute)
                        yield return new Module(group, this, gattribute.Name, gattribute.Aliases);
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
