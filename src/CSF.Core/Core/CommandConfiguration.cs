using CSF.Helpers;
using CSF.TypeReaders;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace CSF.Core
{
    public class CommandConfiguration
    {
        private Assembly[] _assemblies = [ Assembly.GetExecutingAssembly() ];

        public Assembly[] Assemblies
        {
            get
            {
                return _assemblies;
            }
        }

        private TypeReader[] _typeReaders = [];

        public TypeReader[] TypeReaders
        {
            get
            {
                return _typeReaders;
            }
        }

        private ResultResolver _resultResolver = ResultResolver.Default;

        public ResultResolver ResultResolver
        {
            get
            {
                return _resultResolver;
            }
        }

        public TaskAwaitOptions ExecutionPattern { get; set; }

        public CommandConfiguration WithAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                ThrowHelpers.InvalidArg(assemblies);
            }

            _assemblies = assemblies;

            return this;
        }

        public CommandConfiguration AddAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                ThrowHelpers.InvalidArg(assembly);
            }

            if (_assemblies.Contains(assembly))
            {
                ThrowHelpers.NotDistinct(assembly);
            }

            AddAsm(assembly);

            return this;
        }

        public CommandConfiguration TryAddAssembly(Assembly assembly)
        {
            if (assembly != null)
            {
                if (!_assemblies.Contains(assembly))
                {
                    AddAsm(assembly);
                }
            }

            return this;
        }

        public CommandConfiguration WithTypeReaders(params TypeReader[] typeReaders)
        {
            if (typeReaders == null)
            {
                ThrowHelpers.InvalidArg(typeReaders);
            }

            _typeReaders = typeReaders.Distinct(TypeReaderEqualityComparer.Default).ToArray();

            return this;
        }

        public CommandConfiguration AddTypeReader(TypeReader typeReader)
        {
            if (typeReader == null)
            {
                ThrowHelpers.InvalidArg(typeReader);
            }

            if (_typeReaders.Contains(typeReader, TypeReaderEqualityComparer.Default))
            {
                ThrowHelpers.NotDistinct(typeReader);
            }

            AddTr(typeReader);

            return this;
        }

        public CommandConfiguration TryAddTypeReader(TypeReader typeReader)
        {
            if (typeReader != null)
            {
                if (!_typeReaders.Contains(typeReader, TypeReaderEqualityComparer.Default))
                {
                    AddTr(typeReader);
                }
            }

            return this;
        }

        public CommandConfiguration ConfigureResultAction([DisallowNull] Func<ICommandContext, ICommandResult, IServiceProvider, Task> configureDelegate)
        {
            if (configureDelegate == null)
            {
                ThrowHelpers.InvalidArg(configureDelegate);
            }

            _resultResolver = new ResultResolver(configureDelegate);

            return this;
        }

        private void AddAsm(Assembly assembly)
        {
            var oLen = _assemblies.Length;
            Array.Resize(ref _assemblies, oLen);

            _assemblies[oLen] = assembly;
        }

        private void AddTr(TypeReader typeReader)
        {
            var oLen = _typeReaders.Length;
            Array.Resize(ref _typeReaders, oLen);

            _typeReaders[oLen] = typeReader;
        }
    }
}
