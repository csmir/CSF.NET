using CSF.Helpers;
using CSF.TypeReaders;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CSF.Core
{
    /// <summary>
    ///     Represents the configuration to be implemented by a new <see cref="CommandManager"/>.
    /// </summary>
    public class CommandConfiguration
    {
        private Assembly[] _assemblies = [Assembly.GetExecutingAssembly()];

        /// <summary>
        ///     Gets a collection of assemblies that the <see cref="CommandManager"/> will use to register commands.
        /// </summary>
        /// <remarks>
        ///     If the collection has not been altered, it will include <see cref="Assembly.GetEntryAssembly"/> as the default value.
        /// </remarks>
        public Assembly[] Assemblies
        {
            get
            {
                return _assemblies;
            }
        }

        private TypeReader[] _typeReaders = [];

        /// <summary>
        ///     Gets a collection of <see cref="TypeReader"/>'s that the <see cref="CommandManager"/> will use to handle unknown argument types.
        /// </summary>
        /// <remarks>
        ///     It is adviced <b>not</b> to create new implementations of <see cref="TypeReader"/> without first confirming if the target type is not already supported. 
        ///     All valuetypes and time/date types are already supported out of the box.
        /// </remarks>
        public TypeReader[] TypeReaders
        {
            get
            {
                return _typeReaders;
            }
        }

        private ResultResolver _resultResolver = ResultResolver.Default;

        /// <summary>
        ///     Gets or sets a resolver that contains a handler for handling command post-execution data.
        /// </summary>
        public ResultResolver ResultResolver
        {
            get
            {
                return _resultResolver;
            }
            set
            {
                if (value == null)
                {
                    ThrowHelpers.InvalidArg(value);
                }

                _resultResolver = value;
            }
        }

        private AsyncApproach _asyncApproach = AsyncApproach.Default;

        /// <summary>
        ///     Gets or sets the approach to asynchronousity in commands.
        /// </summary>
        /// <remarks>
        ///     If set to <see cref="AsyncApproach.Await"/>, the manager will wait for a command to finish before allowing another to be executed.
        ///     If set to <see cref="AsyncApproach.Discard"/>, the manager will seperate the command execution from the entry stack, and slip it to another thread. 
        ///     Only change this value if you have read the documentation of <see cref="Core.AsyncApproach"/> and understand the definitions.
        /// </remarks>
        public AsyncApproach AsyncApproach
        {
            get
            {
                return _asyncApproach;
            }
            set
            {
                if (value is not AsyncApproach.Await or AsyncApproach.Discard)
                {
                    ThrowHelpers.InvalidOp("AsyncApproach does not support values that exceed the provided options, ranging between 0 and 1.");
                }

                _asyncApproach = value;
            }
        }

        /// <summary>
        ///     Replaces the existing values in <see cref="Assemblies"/> with a new collection.
        /// </summary>
        /// <remarks>
        ///     To prevent duplicate value recognition, <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> is called to remove duplicates from <paramref name="assemblies"/>.
        /// </remarks>
        /// <param name="assemblies">A collection of <see cref="Assembly"/> targets to be used to register commands.</param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
        public CommandConfiguration WithAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                ThrowHelpers.InvalidArg(assemblies);
            }

            _assemblies = assemblies.Distinct().ToArray();

            return this;
        }

        /// <summary>
        ///     Adds an <see cref="Assembly"/> to <see cref="Assemblies"/>.
        /// </summary>
        /// <remarks>
        ///     This call will throw if <see cref="Assemblies"/> already contains a value for <paramref name="assembly"/>.
        /// </remarks>
        /// <param name="assembly">An <see cref="Assembly"/> target to be used to register commands.</param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
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

        /// <summary>
        ///     Attempts to add an <see cref="Assembly"/> to <see cref="Assemblies"/>.
        /// </summary>
        /// <remarks>
        ///     Will not add <paramref name="assembly"/> to <see cref="Assemblies"/> if it already contains the target assembly.
        /// </remarks>
        /// <param name="assembly">An <see cref="Assembly"/> target to be used to register commands.</param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
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

        /// <summary>
        ///     Replaces the existing values in <see cref="TypeReaders"/> with a new collection.
        /// </summary>
        /// <remarks>
        ///     To prevent duplicate value recognition, <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> is called to remove duplicates from <paramref name="typeReaders"/>.
        /// </remarks>
        /// <param name="typeReaders">A collection of <see cref="TypeReader"/>'s to parse unknown argument types.</param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
        public CommandConfiguration WithTypeReaders(params TypeReader[] typeReaders)
        {
            if (typeReaders == null)
            {
                ThrowHelpers.InvalidArg(typeReaders);
            }

            _typeReaders = typeReaders.Distinct(TypeReader.EqualityComparer.Default).ToArray();

            return this;
        }

        /// <summary>
        ///     Adds a <see cref="TypeReader"/> to <see cref="TypeReaders"/>.
        /// </summary>
        /// <remarks>
        ///     This call will throw if <see cref="TypeReaders"/> already contains an implementation of the target type, validated by a custom equality comparer.
        /// </remarks>
        /// <param name="typeReader">A <see cref="TypeReader"/> to parse unknown argument types.</param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
        public CommandConfiguration AddTypeReader(TypeReader typeReader)
        {
            if (typeReader == null)
            {
                ThrowHelpers.InvalidArg(typeReader);
            }

            if (_typeReaders.Contains(typeReader, TypeReader.EqualityComparer.Default))
            {
                ThrowHelpers.NotDistinct(typeReader);
            }

            AddTr(typeReader);

            return this;
        }

        /// <summary>
        ///     Attempts to add a <see cref="TypeReader"/> to <see cref="TypeReaders"/>.
        /// </summary>
        /// <remarks>
        ///     Will not add <paramref name="typeReader"/> to <see cref="TypeReaders"/> if it already contains an implementation of the target type, validated by a custom equality comparer.
        /// </remarks>
        /// <param name="typeReader">A <see cref="TypeReader"/> to parse unknown argument types.</param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
        public CommandConfiguration TryAddTypeReader(TypeReader typeReader)
        {
            if (typeReader != null)
            {
                if (!_typeReaders.Contains(typeReader, TypeReader.EqualityComparer.Default))
                {
                    AddTr(typeReader);
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns>The same <see cref="CommandConfiguration"/> for call chaining.</returns>
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
