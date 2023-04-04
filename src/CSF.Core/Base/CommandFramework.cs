using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace CSF
{
    public class CommandFramework
    {
        private readonly bool _asyncExec;

        private readonly ComponentContainer _components;
        private readonly TypeReaderContainer _typeReaders;

        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        public CommandFramework(IServiceProvider serviceProvider)
            : this(serviceProvider, serviceProvider.GetRequiredService<ILogger<CommandFramework>>())
        {

        }

        public CommandFramework(IServiceProvider serviceProvider, ILogger logger)
        {
            _logger = logger;
            _services = serviceProvider;

            _components = serviceProvider.GetRequiredService<ComponentContainer>();
            _typeReaders = serviceProvider.GetRequiredService<TypeReaderContainer>();

            _asyncExec = serviceProvider.GetRequiredService<FrameworkBuilderContext>()
                .DoAsynchronousExecution;
        }

        public virtual async Task<IResult> TryExecuteAsync<T>(T context, IServiceScope scope = null, CancellationToken cancellationToken = default)
            where T : IContext
        {
            var services = scope?.ServiceProvider ?? _services;

            if (!_asyncExec)
            {
                var result = await RunPipelineAsync(context, services, cancellationToken);

                await AfterExecuteAsync(context, services, result);

                return result;
            }

            _ = Task.Run(async () =>
            {
                var result = await RunPipelineAsync(context, services, cancellationToken);

                await AfterExecuteAsync(context, services, result);
            });

            return ExecuteResult.FromSuccess();
        }

        public virtual async ValueTask<IResult> RunPipelineAsync<T>(T context, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            var searchResult = await SearchAsync(context, cancellationToken);

            if (!searchResult.IsSuccess)
                return searchResult;

            var checkResult = await CheckAsync(context, searchResult.Result, provider, cancellationToken);

            if (!checkResult.IsSuccess)
                return checkResult;

            var (command, args) = ((CheckYieldResult)checkResult).Result.First();

            var result = await command.ExecuteAsync(context, args.Result, provider, cancellationToken);

            return result;
        }

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

                    var result = await groups[0].SearchAsync(context, cancellationToken);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return SearchResult.FromSuccess(commands);

            return OnCommandNotFound(context);
        }

        protected virtual SearchResult OnCommandNotFound<TContext>(TContext context)
            where TContext : IContext
            => SearchResult.FromError($"Failed to find command with name: {context.Name}.");

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
                var preconResult = await command.CheckAsync(context, provider, cancellationToken);

                if (preconResult.IsSuccess)
                {
                    var readResult = await command.ReadAsync(context, _typeReaders, cancellationToken);

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

                    if (command.MaxLength == contextLength)
                        yield return command;

                    if (command.MaxLength <= contextLength)
                        foreach (var parameter in command.Parameters)
                            if (parameter.Flags.HasFlag(ParameterFlags.Remainder))
                                yield return command;

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
                    return OnBestOverloadUnavailable(context);
                else
                    return CheckResult.FromSuccess(new[] { overload });
            }

            _logger.LogTrace("Found matches for name: '{}': {}", context.Name, string.Join(", ", matches));

            return CheckResult.FromSuccess(matches);
        }

        protected virtual CheckResult OnBestOverloadUnavailable<TContext>(TContext context)
            where TContext : IContext
            => CheckResult.FromError($"Failed to find overload that best matches input: {context.Name}.");

        public virtual ValueTask AfterExecuteAsync<TContext>(TContext context, IServiceProvider services, IResult result)
            where TContext : IContext
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
