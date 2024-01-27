using CSF.Helpers;
using CSF.Preconditions;
using CSF.TypeReaders;

namespace CSF.Reflection
{

    public sealed class ModuleInfo : IConditional
    {

        public string Name { get; }


        public string[] Aliases { get; }


        public Attribute[] Attributes { get; }


        public PreconditionAttribute[] Preconditions { get; }


        public bool HasPreconditions { get; }


        public IConditional[] Components { get; }


        public Type Type { get; }


        public ModuleInfo Root { get; }

        internal ModuleInfo(Type type, IDictionary<Type, TypeReader> typeReaders, ModuleInfo root = null, string expectedName = null, string[] aliases = null)
        {
            var attributes = type.GetAttributes(true);
            var preconditions = attributes.GetPreconditions();

            Root = root;
            Type = type;

            Attributes = attributes;
            Preconditions = preconditions;
            HasPreconditions = preconditions.Length > 0;

            Components = this.GetComponents(typeReaders);

            Name = expectedName ?? type.Name;
            Aliases = aliases ?? [Name];
        }


        public override string ToString()
            => $"{(Root != null ? $"{Root}." : "")}{(Type.Name != Name ? $"{Type.Name}['{Name}']" : $"{Name}")}";
    }
}
