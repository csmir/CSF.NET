using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace CSF
{
    public class CommandFramework : ICommandFramework
    {
        private readonly bool _asyncExec;

        private readonly ICommandConveyor _conveyor;
        private readonly ComponentContainer _components;
        private readonly ILogger<CommandFramework> _logger;
        private readonly IServiceProvider _services;

        public CommandFramework(
            ComponentContainer components,
            FrameworkBuilderContext context,
            IServiceProvider serviceProvider,
            ILogger<CommandFramework> logger,
            ICommandConveyor conveyor)
        {
            _logger = logger;
            _conveyor = conveyor;
            _services = serviceProvider;
            _components = components;

            _asyncExec = context.DoAsynchronousExecution;
        }

        /// <inheritdoc/>
        public virtual async Task<IResult> ExecuteAsync<T>(T context, IServiceScope scope = null, CancellationToken cancellationToken = default)
            where T : IContext
        {
            var services = scope?.ServiceProvider ?? _services;

            if (!_asyncExec)
            {
                var result = await RunPipelineAsync(context, services, cancellationToken);

                await _conveyor.OnCommandExecuted(context, services, result);

                return result;
            }

            _ = Task.Run(async () =>
            {
                var result = await RunPipelineAsync(context, services, cancellationToken);

                await _conveyor.OnCommandExecuted(context, services, result);
            });
            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual async ValueTask<IResult> RunPipelineAsync<T>(T context, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            _logger.LogDebug($"Starting command pipeline for name: '{context.Name}'");

            var searchResult = await SearchAsync(context, cancellationToken);

            if (!searchResult.IsSuccess)
                return searchResult;

            var checkResult = await CheckAsync(context, searchResult.Result, provider, cancellationToken);

            if (!checkResult.IsSuccess)
                return checkResult;

            var (command, args) = ((CheckYieldResult)checkResult).Result.First();

            var constructResult = await ConstructAsync(context, command, provider, cancellationToken);

            if (!constructResult.IsSuccess)
                return constructResult;

            var result = await ExecuteAsync(context, command, constructResult.Result, args.Result, cancellationToken);

            if (result != null && !result.IsSuccess)
                return result;

            return ExecuteResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual async ValueTask<SearchResult> SearchAsync<T>(T context, CancellationToken cancellationToken)
            where T : IContext
        {
            var matches = _components.Values
                .Where(component => component.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.CastWhere<Module>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.CastWhere<Command>()
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

                    var result = await SearchModuleAsync(context, groups[0], cancellationToken);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return SearchResult.FromSuccess(commands);

            return _conveyor.OnCommandNotFound(context);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<SearchResult> SearchModuleAsync<T>(T context, Module module, CancellationToken cancellationToken)
            where T : IContext
        {
            var matches = module.Components
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

                    return await SearchModuleAsync(context, groups[0], cancellationToken);
                }

                else
                    return _conveyor.OnCommandNotFound(context);
            }

            return SearchResult.FromSuccess(commands);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<IResult> CheckAsync<T>(T context, IEnumerable<Command> commands, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            IResult failureResult = null;

            var matchResult = await CheckMatchesAsync(context, commands, cancellationToken);

            if (!matchResult.IsSuccess)
                return matchResult;

            commands = matchResult.Result;

            var matches = new List<(Command, ArgsResult)>();

            foreach (var command in commands)
            {
                var preconResult = await CheckPreconditionsAsync(context, command, provider, cancellationToken);

                if (preconResult.IsSuccess)
                {
                    var readResult = await ReadAsync(context, command, cancellationToken);

                    if (!readResult.IsSuccess)
                        failureResult = readResult;
                    else
                        matches.Add((command, (ArgsResult)readResult));
                }
                else
                    failureResult ??= preconResult;
            }

            if (!matches.Any())
                return CheckResult.FromError(failureResult.ErrorMessage, failureResult.Exception);

            return CheckYieldResult.FromSuccess(matches);
        }

        /// <inheritdoc/>
        public virtual ValueTask<CheckResult> CheckMatchesAsync<T>(T context, IEnumerable<Command> commands, CancellationToken cancellationToken)
            where T : IContext
        {
            Command overload = null;
            IEnumerable<Command> SearchMatches()
            {
                foreach (var command in commands)
                {
                    if (overload is null && command.IsErrorOverload)
                        overload = command;

                    var min = command.MaxLength;
                    var contextLength = context.Parameters.Count;

                    // If parameter & input length is equal, prefer it over all matches.
                    if (command.MaxLength == contextLength)
                        yield return command;

                    // If command length is lower than context length, look for a remainder attribute.
                    // Due to sorting upwards, it will continue the loop and prefer the remainder attr with most parameters.
                    if (command.MaxLength <= contextLength)
                        foreach (var parameter in command.Parameters)
                            if (parameter.Flags.HasFlag(ParameterFlags.Remainder))
                                yield return command;

                    // If context length is lower than command length, return the command with least optionals.
                    if (command.MaxLength > contextLength)
                    {
                        if (command.MinLength <= contextLength)
                            yield return command;
                    }
                }
            }

            var matches = SearchMatches();

            if (!matches.Any())
            {
                if (overload is null)
                    return _conveyor.OnBestOverloadUnavailable(context);
                else
                    return CheckResult.FromSuccess(new[] { overload });
            }

            _logger.LogTrace("Found matches for name: '{}': {}", context.Name, string.Join(", ", matches));

            return CheckResult.FromSuccess(matches);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<PreconditionResult> CheckPreconditionsAsync<T>(T context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            foreach (var precon in command.Preconditions)
            {
                var result = await precon.CheckAsync(context, command, provider, cancellationToken);

                if (!result.IsSuccess)
                    return result;
            }

            _logger.LogTrace("Succesfully ran precondition checks for {}.", command.Name);

            return PreconditionResult.FromSuccess();
        }

        /// <inheritdoc/>
        public virtual ValueTask<ConstructionResult> ConstructAsync<T>(T context, Command command, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            var module = provider.GetService(command.Module.Type);

            if (module is ModuleBase moduleBase)
            {
                moduleBase.SetContext(context);
                moduleBase.SetInformation(command);

                _logger.LogTrace("Succesfully constructed module for {}", command.Name);

                return ConstructionResult.FromSuccess(moduleBase);
            }
            return _conveyor.OnInvalidModule(context, command.Module);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<IResult> ReadAsync<T>(T context, Command command, CancellationToken cancellationToken)
            where T : IContext
        {
            var result = await ReadContainerAsync(context, 0, command, cancellationToken);

            if (!result.IsSuccess)
                return result;

            return ArgsResult.FromSuccess(((ArgsResult)result).Result, -1);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<IResult> ReadContainerAsync<T>(T context, int index, IParameterContainer container, CancellationToken cancellationToken)
            where T : IContext
        {
            var parameters = new List<object>();

            foreach (var parameter in container.Parameters)
            {
                if (parameter.Flags.HasRemainder())
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (parameter.Flags.HasOptional() && context.Parameters.Count <= index)
                {
                    var missingResult = _conveyor.OnMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return _conveyor.OnOptionalNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return _conveyor.OnMissingReturnedInvalid(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                if (parameter.Flags.HasNullable() && context.Parameters[index] is string str && (str == "null" || str == "nothing"))
                {
                    parameters.Add(null);
                    index++;
                    continue;
                }

                if (parameter is ComplexParameter complexParam)
                {
                    var result = await ReadContainerAsync(context, index, complexParam, cancellationToken);

                    if (!result.IsSuccess)
                        return result;

                    if (result is ArgsResult argsResult)
                    {
                        index = argsResult.Placement;
                        parameters.Add(complexParam.Constructor.EntryPoint.Invoke(argsResult.Result.ToArray()));
                    }
                }

                else if (parameter is BaseParameter normal)
                {
                    var result = await normal.TypeReader.ReadAsync(context, normal, context.Parameters[index], cancellationToken);

                    if (!result.IsSuccess)
                        return result;

                    index++;
                    parameters.Add(result.Result);
                }
            }

            _logger.LogTrace("Succesfully populated parameters. Count: {}", parameters.Count);

            return ArgsResult.FromSuccess(parameters, index);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<IResult> ExecuteAsync<T>(T context, Command command, ModuleBase module, IEnumerable<object> parameters, CancellationToken cancellationToken)
            where T : IContext
        {
            try
            {
                _logger.LogDebug("Starting execution of {}.", module.CommandInfo.Name);
                var sw = Stopwatch.StartNew();

                await module.BeforeExecuteAsync(command, cancellationToken);

                var returnValue = command.Method.Invoke(module, parameters.ToArray());

                await module.AfterExecuteAsync(command, cancellationToken);

                sw.Stop();
                _logger.LogDebug("Finished execution of {} in {} ms.", module.CommandInfo.Name, sw.ElapsedMilliseconds);

                var result = default(IResult);

                switch (returnValue)
                {
                    case Task<IResult> execTask:
                        result = await execTask;
                        break;
                    case Task task:
                        await task;
                        result = ExecuteResult.FromSuccess();
                        break;
                    case IResult syncResult:
                        if (!syncResult.IsSuccess)
                            return syncResult;
                        break;
                    default:
                        if (returnValue is null)
                            break;
                        result = _conveyor.OnUnhandledReturnType(context, returnValue);
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                return _conveyor.OnUnhandledException(context, command, ex);
            }
        }
    }
}
