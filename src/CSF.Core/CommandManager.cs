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
    /// <summary>
    ///     The root class of CSF, responsible for managing commands and their execution. 
    /// </summary>
    public class CommandManager
    {
        private readonly ComponentContainer _components;
        private readonly TypeReaderContainer _typeReaders;

        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        /// <summary>
        ///     Creates a new <see cref="CommandManager"/>.
        /// </summary>
        /// <param name="serviceProvider">The serviceprovider used to request registered services.</param>
        public CommandManager(IServiceProvider serviceProvider)
            : this(serviceProvider, serviceProvider.GetRequiredService<ILogger<CommandManager>>())
        {

        }

        /// <summary>
        ///     Creates a new <see cref="CommandManager"/>.
        /// </summary>
        /// <param name="serviceProvider">The serviceprovider used to request registered services.</param>
        /// <param name="logger">The logger to report the execution process.</param>
        public CommandManager(IServiceProvider serviceProvider, ILogger logger)
        {
            _logger = logger;
            _services = serviceProvider;

            _components = serviceProvider.GetRequiredService<ComponentContainer>();
            _typeReaders = serviceProvider.GetRequiredService<TypeReaderContainer>();
        }

        /// <summary>
        ///     Attempts to execute a command from the provided context.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="scope">The service scope to use for command execution. If this property is ignored, it will use the default provider used to populate the manager.</param>
        /// <param name="async">If execution should run asynchronously. To get a result from async execution, override <see cref="AfterExecuteAsync(IContext, IServiceProvider, IResult)"/>.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public virtual async ValueTask<IResult> TryExecuteAsync<T>(T context, IServiceScope scope = null, bool async = false, CancellationToken cancellationToken = default)
            where T : IContext
        {
            var services = scope?.ServiceProvider ?? _services;

            if (!async)
                return await ExecutePipelineAsync(context, services, cancellationToken);

            _ = Task.Run(async ()
                => await ExecutePipelineAsync(context, services, cancellationToken));

            return ExecuteResult.FromSuccess();
        }

        /// <summary>
        ///     Executes the pipeline that searches, matches and processes a command from the context provided in <see cref="TryExecuteAsync{T}(T, IServiceScope, bool, CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="services">The services used to execute the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        protected virtual async ValueTask<IResult> ExecutePipelineAsync<T>(T context, IServiceProvider services, CancellationToken cancellationToken)
            where T : IContext
        {
            var searchResult = await SearchAsync(context, cancellationToken);

            if (!searchResult.IsSuccess)
                return searchResult;

            var checkResult = await CheckAsync(context, searchResult.Result, services, cancellationToken);

            if (!checkResult.IsSuccess)
                return checkResult;

            var (command, args) = ((CheckYieldResult)checkResult).Result.First();

            var result = await command.ExecuteAsync(context, args.Result, services, cancellationToken);

            await AfterExecuteAsync(context, services, result);

            return result;
        }

        /// <summary>
        ///     Override this method to modify how command post-execution is handled.
        /// </summary>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="services">The services used to execute the command.</param>
        /// <param name="result">The result returned by the execution.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/>.</returns>
        protected virtual ValueTask AfterExecuteAsync(IContext context, IServiceProvider services, IResult result)
            => new ValueTask(Task.CompletedTask);

        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
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

        /// <summary>
        ///     Returns the error that displays that no command was found for the provided context.
        /// </summary>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <returns>The result of the executed command.</returns>
        protected virtual SearchResult OnCommandNotFound(IContext context)
            => SearchResult.FromError($"Failed to find command with name: {context.Name}.");

        /// <summary>
        ///     Checks the search matches to discover the best overload for execution.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="commands">The commands to grade for their match validity.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public virtual ValueTask<CheckResult> MatchAsync<T>(T context, IEnumerable<Command> commands, CancellationToken cancellationToken)
            where T : IContext
        {
            Command overload = null;

            IEnumerable<Command> Yield()
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

            var matches = Yield();

            if (!matches.Any())
            {
                if (overload is null)
                    return OnBestOverloadUnavailable(context);

                return CheckResult.FromSuccess(new[] { overload });
            }

            _logger.LogTrace("Found matches for name: '{}': {}", context.Name, string.Join(", ", matches));

            return CheckResult.FromSuccess(matches);
        }

        /// <summary>
        ///     Returns the error that displays that no best match exists for the command input.
        /// </summary>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <returns>The result of the executed command.</returns>
        protected virtual CheckResult OnBestOverloadUnavailable(IContext context)
            => CheckResult.FromError($"Failed to find overload that best matches input: {context.Name}.");

        /// <summary>
        ///     Checks the search matches to discover the best overload for execution and grades them based on their typereader and precondition performance.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="commands">The commands to grade for their match validity.</param>
        /// <param name="services">The services used to execute the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public virtual async ValueTask<IResult> CheckAsync<T>(T context, IEnumerable<Command> commands, IServiceProvider services, CancellationToken cancellationToken)
            where T : IContext
        {
            IResult failureResult = null;

            var matchResult = await MatchAsync(context, commands, cancellationToken);

            if (!matchResult.IsSuccess)
                return matchResult;

            commands = matchResult.Result;

            var matches = new List<(Command, ParseResult)>();

            foreach (var command in commands)
            {
                var preconResult = await command.CheckAsync(context, services, cancellationToken);

                if (preconResult.IsSuccess)
                {
                    var readResult = await command.ReadAsync(context, _typeReaders, cancellationToken);

                    if (!readResult.IsSuccess)
                        failureResult = readResult;
                    else
                        matches.Add((command, (ParseResult)readResult));
                }
                else
                    failureResult ??= preconResult;
            }

            if (!matches.Any())
                return CheckResult.FromError(failureResult.ErrorMessage, failureResult.Exception);

            return CheckYieldResult.FromSuccess(matches);
        }
    }
}
