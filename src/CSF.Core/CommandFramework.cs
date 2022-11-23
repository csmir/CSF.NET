using CSF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace CSF
{
    /// <summary>
    ///     Represents the handler for registered commands.
    /// </summary>
    public partial class CommandFramework
    {
        /// <summary>
        ///     The range of registered commands.
        /// </summary>
        public List<IConditionalComponent> CommandMap { get; }

        /// <summary>
        ///     The configuration for this service.
        /// </summary>
        public CommandConfiguration Configuration { get; }

        private ILogger _logger;
        /// <summary>
        ///     The logger passed throughout the build and execution process.
        /// </summary>
        /// <remarks>
        ///     The resolver behind this logger is available for modification in <see cref="ConfigureLogger"/>.
        /// </remarks>
        public ILogger Logger
        {
            get
            {
                if (_logger is null)
                    _logger = ConfigureLogger();
                return _logger;
            }
        }

        private readonly AsyncEvent<Func<IContext, IResult, Task>> _commandExecuted;
        /// <summary>
        ///     Invoked when a command is executed.
        /// </summary>
        /// <remarks>
        ///     This is the only way to do post-execution processing when <see cref="CommandConfiguration.DoAsynchronousExecution"/> is set to <see cref="true"/>.
        /// </remarks>
        public event Func<IContext, IResult, Task> CommandExecuted
        {
            add
                => _commandExecuted.Add(value);
            remove
                => _commandExecuted.Remove(value);
        }

        private readonly AsyncEvent<Func<IConditionalComponent, Task>> _commandRegistered;
        /// <summary>
        ///     Invoked when a command is registered.
        /// </summary>
        /// <remarks>
        ///     This event can be used to do additional registration steps for certain services.
        /// </remarks>
        public event Func<IConditionalComponent, Task> CommandRegistered
        {
            add
                => _commandRegistered.Add(value);
            remove
                => _commandRegistered.Remove(value);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework"/> with default configuration.
        /// </summary>
        public CommandFramework()
            : this(new CommandConfiguration())
        {

        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework"/> with provided configuration.
        /// </summary>
        /// <param name="config"></param>
        public CommandFramework(CommandConfiguration config)
        {
            CommandMap = new List<IConditionalComponent>();

            if (config.TypeReaders is null)
                config.TypeReaders = new TypeReaderProvider();

            if (config.Prefixes is null)
                config.Prefixes = new PrefixProvider();

            Configuration = config;

            _commandRegistered = new AsyncEvent<Func<IConditionalComponent, Task>>();
            _commandExecuted = new AsyncEvent<Func<IContext, IResult, Task>>();
        }

        /// <summary>
        ///     Configures the application logger and exposes it within the pipeline.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify what will be registered.
        /// </remarks>
        /// <returns></returns>
        protected virtual ILogger ConfigureLogger()
        {
            return new DefaultLogger(Configuration.DefaultLogLevel);
        }

        /// <summary>
        ///     Registers all command modules in the provided assembly to the <see cref="CommandMap"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the registration flow.
        /// </remarks>
        /// <param name="assembly">The assembly to find all modules for.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual async Task BuildModulesAsync(Assembly assembly)
        {
            if (assembly is null)
            {
                Logger.WriteError("Expected a not-null value.", new ArgumentNullException(nameof(assembly)));
                return;
            }

            var types = assembly.GetTypes();

            var baseType = typeof(IModuleBase);

            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsNested)
                {
                    Logger.WriteTrace($"Found module by name: {type.Name}.");
                    await BuildModuleAsync(type);
                }
            }
        }

        /// <summary>
        ///     Registers a single module to the <see cref="CommandMap"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the registration flow.
        /// </remarks>
        /// <param name="type">The <see cref="ModuleBase{T}"/> to register.</param>
        /// <returns>An asynchronous <see cref="Task"/> with no return type.</returns>
        public virtual async Task BuildModuleAsync(Type type)
        {
            var module = new Module(Configuration, type);

            foreach (var component in module.Components)
            {
                try
                {
                    if (Configuration.InvokeOnlyNameRegistrations)
                    {
                        if (!CommandMap.Any(x => x.Aliases.SequenceEqual(component.Aliases)))
                        {
                            await _commandRegistered.InvokeAsync(component);
                            Logger.WriteTrace($"Invoked registration event for {component.Name}");
                        }
                    }
                    else
                    {
                        await _commandRegistered.InvokeAsync(component);
                        Logger.WriteTrace($"Invoked registration event for {component.Name}");
                    }

                    CommandMap.Add(component);
                    Logger.WriteDebug($"Registered item: {component.GetQualifiedNames()}.");
                }
                catch (Exception ex)
                {
                    Logger.WriteCritical(ex.Message, ex);
                    return;
                }
                CommandMap.Add(component);
            }
        }

        /// <summary>
        ///     Tries to parse an <see cref="IPrefix"/> from the provided raw input and will remove the length of the prefix from it.
        /// </summary>
        /// <remarks>
        ///     This method will browse the <see cref="PrefixProvider"/> from the <see cref="Configuration"/> of this instance.
        /// </remarks>
        /// <param name="rawInput">The raw text input to try and parse a prefix for.</param>
        /// <param name="prefix">The resulting prefix. <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a matching <see cref="IPrefix"/> was found in the <see cref="PrefixProvider"/>. <see langword="false"/> if not.</returns>
        public bool TryParsePrefix(ref string rawInput, out IPrefix prefix)
        {
            if (Configuration.Prefixes.TryGetPrefix(rawInput, out prefix))
            {
                rawInput = rawInput.Substring(prefix.Value.Length).TrimStart();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Tries to execute a command with provided <see cref="IContext"/>.
        /// </summary>
        /// <remarks>
        ///     If <see cref="CommandConfiguration.DoAsynchronousExecution"/> is enabled, the <see cref="IResult"/> of this method will always return success.
        ///     Use the <see cref="CommandExecuted"/> event to do post-execution processing.
        ///     <br/><br/>
        ///     If you want to change the order of execution or add extra steps, override <see cref="RunPipelineAsync{T}(T, IServiceProvider)"/>.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public async Task<IResult> ExecuteCommandAsync<T>(T context, IServiceProvider provider = null)
            where T : IContext
        {
            if (provider is null)
                provider = EmptyServiceProvider.Instance;

            if (Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, provider);
                    await _commandExecuted.InvokeAsync(context, result);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, provider);
                await _commandExecuted.InvokeAsync(context, result);
                return result;
            }
        }
    }
}
