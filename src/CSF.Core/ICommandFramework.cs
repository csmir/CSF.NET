using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the default implementation of the <see cref="CommandFramework{T}"/>.
    /// </summary>
    public interface ICommandFramework
    {
        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public IList<IConditionalComponent> Commands { get; }

        /// <summary>
        ///     The range of registered typereaders.
        /// </summary>
        public TypeReaderProvider TypeReaders { get; }

        /// <summary>
        ///     The range of registered prefixes.
        /// </summary>
        public PrefixProvider Prefixes { get; }

        /// <summary>
        ///     The range of registered result handlers.
        /// </summary>
        public ResultHandlerProvider ResultHandlers { get; }

        /// <summary>
        ///     The logger passed throughout the build and execution process.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        ///     Builds all modules in the provided assemblies in <see cref="CommandConfiguration.RegistrationAssemblies"/>.
        /// </summary>
        public void BuildModuleAssemblies();

        /// <summary>
        ///     Builds all modules in the provided <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly"></param>
        public void BuildModuleAssembly(Assembly assembly = null);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void BuildModule(Type type);

        /// <summary>
        ///     Tries to parse an <see cref="IPrefix"/> from the provided raw input and will remove the length of the prefix from it.
        /// </summary>
        /// <remarks>
        ///     This method will browse the <see cref="PrefixProvider"/> from the <see cref="Configuration"/> of this instance.
        /// </remarks>
        /// <param name="rawInput">The raw text input to try and parse a prefix for.</param>
        /// <param name="prefix">The resulting prefix. <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a matching <see cref="IPrefix"/> was found in the <see cref="PrefixProvider"/>. <see langword="false"/> if not.</returns>
        public bool TryParsePrefix(ref string rawInput, out IPrefix prefix);

        /// <summary>
        ///     Tries to execute a command with provided <see cref="IContext"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="CommandConfiguration.DoAsynchronousExecution"/> is enabled, the <see cref="IResult"/> of this method will always return success.
        ///     Use the <see cref="CommandExecuted"/> event to do post-execution processing.
        ///     <br/><br/>
        ///     If you want to change the order of execution or add extra steps, override <see cref="RunPipelineAsync{T}(T, IServiceProvider)"/>.
        /// </remarks>
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public Task<IResult> ExecuteCommandAsync<TContext>(TContext context, IServiceProvider provider = null)
            where TContext : IContext;
    }
}
