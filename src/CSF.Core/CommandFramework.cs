using CSF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace CSF
{
    /// <summary>
    ///     Represents the handler for registered commands.
    /// </summary>
    public sealed class CommandFramework<T> : ICommandFramework
        where T : ImplementationFactory, new()
    {
        /// <summary>
        ///     The configurator that handles command creation.
        /// </summary>
        public T Configurator { get; }

        /// <inheritdoc/>
        public IList<IConditionalComponent> Commands { get; }

        private TypeReaderProvider _typeReaders;
        /// <inheritdoc/>
        public TypeReaderProvider TypeReaders
        {
            get
            {
                _typeReaders ??= Configurator.ConfigureTypeReaders();
                return _typeReaders;
            }
        }

        private PrefixProvider _prefixes;
        /// <inheritdoc/>
        public PrefixProvider Prefixes
        {
            get
            {
                _prefixes ??= Configurator.ConfigurePrefixes();
                return _prefixes;
            }
        }

        private ResultHandlerProvider _resultHandlers;
        /// <inheritdoc/>
        public ResultHandlerProvider ResultHandlers
        {
            get
            {
                _resultHandlers ??= Configurator.ConfigureResultHandlers();
                return _resultHandlers;
            }
        }

        private ILogger _logger;
        /// <inheritdoc/>
        public ILogger Logger
        {
            get
            {
                _logger ??= Configurator.ConfigureLogger();
                return _logger;
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/> with provided configuration.
        /// </summary>
        /// <param name="config"></param>
        public CommandFramework()
            : this(new())
        {

        }

        /// <summary>
        ///     Creates a new instance of <see cref="CommandFramework{T}"/> with provided configuration.
        /// </summary>
        /// <param name="config"></param>
        public CommandFramework(T implementationFactory)
        {
            Configurator = implementationFactory;
            Commands = new List<IConditionalComponent>();

            if (Configurator.Configuration.AutoRegisterAssemblies)
                BuildModuleAssemblies();
        }

        public void BuildModuleAssemblies()
        {
            foreach (var assembly in Configurator.Configuration.RegistrationAssemblies)
            {
                BuildModuleAssembly(assembly);
            }
        }

        public void BuildModuleAssembly(Assembly assembly = null)
        {
            assembly ??= Assembly.GetEntryAssembly();

            var mt = typeof(IModuleBase);

            foreach (var type in assembly.ExportedTypes)
            {
                if (mt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                {
                    BuildModule(type);
                    return;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void BuildModule(Type type)
        {
            var module = ModuleInfo.Build(TypeReaders, type);

            foreach (var component in module.Components)
                Commands.Add(component);
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
            if (Prefixes.TryGetPrefix(rawInput, out prefix))
            {
                rawInput = rawInput[prefix.Value.Length..].TrimStart();
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
        /// <typeparam name="TContext">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        public async Task<IResult> ExecuteCommandAsync<TContext>(TContext context, IServiceProvider provider = null)
            where TContext : IContext
        {
            provider ??= Configurator.Provider
                ?? EmptyServiceProvider.Instance;

            if (Configurator.Configuration.DoAsynchronousExecution)
            {
                _ = Task.Run(async () =>
                {
                    var result = await RunPipelineAsync(context, provider);

                    await ResultHandlers.InvokeHandlers(context, result);
                });
                return ExecuteResult.FromSuccess();
            }

            else
            {
                var result = await RunPipelineAsync(context, provider);

                await ResultHandlers.InvokeHandlers(context, result);

                return result;
            }
        }

        private async Task<IResult> RunPipelineAsync<TContext>(TContext context, IServiceProvider provider)
            where TContext : IContext
        {
            Logger.WriteDebug($"Starting command pipeline for name: '{context.Name}'");

            var searchResult = Search(context);
            if (!searchResult.IsSuccess)
                return searchResult;

            var command = searchResult.Match;

            var preconResult = await CheckPreconditionsAsync(context, command, provider);
            if (!preconResult.IsSuccess)
                return preconResult;

            var constructResult = Construct(context, command, provider);
            if (!constructResult.IsSuccess)
                return constructResult;

            var readResult = await ParseAsync(context, command);
            if (!readResult.IsSuccess)
                return readResult;

            return await ExecuteAsync(
                context: context,
                command: command,
                commandBase: (ModuleBase<TContext>)constructResult.Result,
                parameters: ((ParseResult)readResult).Result.ToArray());
        }

        private SearchResult Search<TContext>(TContext context)
            where TContext : IContext
        {
            var matches = Commands
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.SelectWhere<ModuleInfo>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.SelectWhere<CommandInfo>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (groups.Any())
            {
                if (context.Parameters.Count > 0)
                {
                    var oldName = context.Name;
                    var oldParam = context.Parameters;

                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    var result = SearchModule(context, groups[0]);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return SearchCommands(context, commands);

            return Configurator.CommandNotFoundResult(context);
        }

        private SearchResult SearchModule<TContext>(TContext context, ModuleInfo module)
            where TContext : IContext
        {
            var matches = module.Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.SelectWhere<CommandInfo>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.SelectWhere<ModuleInfo>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return SearchModule(context, groups[0]);
                }

                else
                    return Configurator.CommandNotFoundResult(context);
            }

            return SearchCommands(context, commands);
        }

        private SearchResult SearchCommands<TContext>(TContext context, IEnumerable<CommandInfo> commands)
            where TContext : IContext
        {
            CommandInfo match = null;
            CommandInfo errormatch = null;
            foreach (var command in commands)
            {
                if (errormatch is null && command.IsErrorOverload)
                    errormatch = command;

                var commandLength = command.Parameters.Count;
                var contextLength = context.Parameters.Count;

                // If parameter & input length is equal, prefer it over all matches & exit the loop.
                if (commandLength == contextLength)
                {
                    match = command;
                    break;
                }

                // If command length is lower than context length, look for a remainder attribute.
                // Due to sorting upwards, it will continue the loop and prefer the remainder attr with most parameters.
                if (commandLength <= contextLength)
                {
                    foreach (var parameter in command.Parameters)
                    {
                        if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                        {
                            match = command;
                            break;
                        }
                    }
                }

                // If context length is lower than command length, return the command with least optionals.
                if (commandLength > contextLength)
                {
                    int requiredLength = 0;
                    foreach (var parameter in command.Parameters)
                    {
                        if (contextLength >= requiredLength)
                        {
                            match = command;
                            break;
                        }

                        if (!parameter.Flags.HasFlag(ParameterFlags.IsOptional))
                        {
                            requiredLength++;
                        }
                    }
                }
            }

            if (match is null)
            {
                if (errormatch is null)
                    return Configurator.NoApplicableOverloadResult(context);
                else
                    return SearchResult.FromSuccess(errormatch);
            }

            Logger.WriteTrace($"Found matching command for name: '{context.Name}': {match.Name}");

            return SearchResult.FromSuccess(match);
        }

        private async Task<PreconditionResult> CheckPreconditionsAsync<TContext>(TContext context, CommandInfo command, IServiceProvider provider)
            where TContext : IContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider);

                if (!result.IsSuccess)
                    return result;
            }

            Logger.WriteTrace($"Succesfully ran precondition checks for {command.Name}.");

            return PreconditionResult.FromSuccess();
        }

        private ConstructionResult Construct<TContext>(TContext context, CommandInfo command, IServiceProvider provider)
            where TContext : IContext
        {
            var services = new List<object>();
            foreach (var dependency in command.Module.Constructor.Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return Configurator.ServiceNotFoundResult(context, dependency);

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.EntryPoint.Invoke(services.ToArray());

            if (obj is not ModuleBase<TContext> commandBase)
                return Configurator.InvalidModuleTypeResult(context, command.Module);

            commandBase.SetContext(context);
            commandBase.SetInformation(command);

            commandBase.SetLogger(Logger);

            Logger.WriteTrace($"Succesfully constructed module for {command.Name}");

            return ConstructionResult.FromSuccess(commandBase);
        }

        private async Task<IResult> ParseAsync<TContext>(TContext context, CommandInfo command)
            where TContext : IContext
        {
            int index = 0;
            var parameters = new List<object>();

            foreach (var parameter in command.Parameters)
            {
                if (parameter.Flags.HasFlag(ParameterFlags.IsRemainder))
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (parameter.Flags.HasFlag(ParameterFlags.IsOptional) && context.Parameters.Count <= index)
                {
                    var missingResult = Configurator.ResolveMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return Configurator.OptionalValueNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return Configurator.MissingOptionalFailedMatch(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                var result = await parameter.TypeReader.ReadAsync(context, parameter, context.Parameters[index]);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            Logger.WriteTrace($"Succesfully populated parameters for {command.Name}. Count: {parameters.Count}");

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        private async Task<IResult> ExecuteAsync<TContext>(TContext context, CommandInfo command, ModuleBase<TContext> commandBase, object[] parameters)
            where TContext : IContext
        {
            try
            {
                Logger.WriteDebug($"Starting execution of {commandBase.CommandInfo.Name}.");
                var sw = Stopwatch.StartNew();

                await commandBase.BeforeExecuteAsync(command, context);

                var returnValue = command.Method.Invoke(commandBase, parameters);

                switch (returnValue)
                {
                    case Task<IResult> execTask:
                        var asyncResult = await execTask;
                        if (!asyncResult.IsSuccess)
                            return asyncResult;
                        break;
                    case Task task:
                        await task;
                        break;
                    case IResult syncResult:
                        if (!syncResult.IsSuccess)
                            return syncResult;
                        break;
                    default:
                        if (returnValue is null)
                            break;

                        var unhandledResult = Configurator.ProcessUnhandledReturnType(context, returnValue);
                        if (!unhandledResult.IsSuccess)
                            return unhandledResult;

                        break;
                }

                await commandBase.AfterExecuteAsync(command, context);

                sw.Stop();
                Logger.WriteDebug($"Finished execution of {commandBase.CommandInfo.Name} in {sw.ElapsedMilliseconds} ms.");

                return ExecuteResult.FromSuccess();
            }
            catch (Exception ex)
            {
                return Configurator.UnhandledExceptionResult(context, command, ex);
            }
        }
    }
}
