using System.Reflection;

namespace CSF
{
    public sealed class CMBuilder
    {
        private bool _disposed = false;

        public HashSet<Assembly> Assemblies { get; set; } = [Assembly.GetEntryAssembly()];

        public HashSet<TypeReader> TypeReaders { get; set; } = [];

        public CMBuilder AddEntryAssembly()
        {
            if (_disposed)
                ThrowHelpers.InvalidOp("This builder cannot be reused.");

            Assemblies.Add(Assembly.GetEntryAssembly());
            return this;
        }

        public CMBuilder AddAssembly(Assembly assembly)
        {
            if (_disposed)
                ThrowHelpers.InvalidOp("This builder cannot be reused.");

            Assemblies.Add(assembly);
            return this;
        }

        public CMBuilder AddTypeReader(TypeReader typeReader)
        {
            if (_disposed)
                ThrowHelpers.InvalidOp("This builder cannot be reused.");

            TypeReaders.Add(typeReader);
            return this;
        }

        public CommandManager Build()
        {
            var typeReaders = TypeReader.CreateDefaultReaders().UnionBy(TypeReaders, x => x.Type).ToDictionary(x => x.Type, x => x);

            if (Assemblies.Count == 0)
                ThrowHelpers.InvalidOp("An assembly has to be present in the builder prior to building the CommandManager.");

            IEnumerable<Module> BuildComponents()
            {
                var rootReader = typeof(TypeReader);
                foreach (var assembly in Assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (rootReader.IsAssignableFrom(type)
                            && !type.IsAbstract
                            && !type.ContainsGenericParameters)
                        {
                            var reader = Activator.CreateInstance(type) as TypeReader;

                            // replace existing typereader with replacement handler
                            if (!typeReaders.TryAdd(reader.Type, reader))
                                typeReaders[reader.Type] = reader;
                        }
                    }
                }

                var rootType = typeof(ModuleBase);
                foreach (var assembly in Assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (rootType.IsAssignableFrom(type)
                            && !type.IsAbstract
                            && !type.ContainsGenericParameters)
                        {
                            yield return new Module(type, typeReaders);
                        }
                    }
                }
            }

            _disposed = true;

            return new(BuildComponents().SelectMany(x => x.Components), [.. Assemblies]);
        }
    }
}
