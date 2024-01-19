using System.Reflection;

namespace CSF
{
    public sealed class FrameworkBuilder
    {
        public List<Assembly> Assemblies { get; set; } = [ Assembly.GetEntryAssembly() ];

        public List<TypeReader> TypeReaders { get; set; } = [];

        public FrameworkBuilder AddAssembly()
        {
            Assemblies.Add(Assembly.GetEntryAssembly());
            return this;
        }

        public FrameworkBuilder AddAssembly(Assembly assembly)
        {
            Assemblies.Add(assembly);
            return this;
        }

        public FrameworkBuilder WithAssemblies(params Assembly[] assemblies)
        {
            Assemblies.AddRange(assemblies);
            return this;
        }

        public FrameworkBuilder AddTypeReader(TypeReader typeReader)
        {
            TypeReaders.Add(typeReader);
            return this;
        }

        public FrameworkBuilder WithTypeReaders(params TypeReader[] typeReaders)
        {
            TypeReaders.AddRange(typeReaders);
            return this;
        }

        public CommandManager Build()
        {
            
        }
    }
}
