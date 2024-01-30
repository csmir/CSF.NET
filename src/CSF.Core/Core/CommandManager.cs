using CSF.Exceptions;
using CSF.Helpers;
using CSF.Reflection;
using CSF.TypeReaders;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

[assembly: CLSCompliant(true)]

namespace CSF.Core
{
    public class CommandManager : IDisposable
    {
        private readonly object _searchLock = new();
        private readonly ResultResolver _resultHandle;

        public IReadOnlySet<IConditional> Components { get; }

        public IServiceProvider Services { get; }

        public TypeReader[] TypeReaders { get; }

        public CommandConfiguration Configuration { get; }

        public CommandManager(IServiceProvider services, CommandConfiguration configuration)
        {
            TypeReaders = configuration.TypeReaders.Distinct(TypeReaderEqualityComparer.Default).ToArray();

            if (configuration.Assemblies == null || configuration.Assemblies.Length == 0)
            {
                ThrowHelpers.InvalidArg(nameof(configuration.Assemblies));
            }

            Components = BuildComponents(configuration)
                .SelectMany(x => x.Components)
                .ToHashSet();

            Services = services;

            _resultHandle = services.GetService<ResultResolver>() ?? ResultResolver.Default;  

            Configuration = configuration;
        }

        public void Execute(ICommandContext context, params object[] args)
            => ExecuteAsync(context, args).Wait();

        public Task ExecuteAsync(ICommandContext context, params object[] args)
            => ExecuteAsync(context, args, cancellationToken: default);

        public async Task ExecuteAsync(ICommandContext context, object[] args, CancellationToken cancellationToken = default)
        {
            switch (Configuration.ExecutionPattern)
            {
                case TaskAwaitOptions.Await:
                    {
                        context.LogDebug("Starting execution. Execution pattern = [Await].");
                        await ExecuteInternalAsync(context, args, cancellationToken);
                    }
                    return;
                case TaskAwaitOptions.Discard:
                    {
                        context.LogDebug("Starting execution. Execution pattern = [Discard].");
                        _ = ExecuteInternalAsync(context, args, cancellationToken);
                    }
                    return;
            }
        }

        public IEnumerable<SearchResult> Search(object[] args)
        {
            // recursively search for commands in the execution.
            lock (_searchLock)
            {
                return Components.RecursiveSearch(args, 0);
            }
        }

        #region Executing
        private async Task ExecuteInternalAsync(ICommandContext context, object[] args, CancellationToken cancellationToken)
        {
            var searches = Search(args);

            var c = 0;

            foreach (var search in searches.OrderByDescending(x => x.Command.Priority))
            {
                c++;

                var match = await MatchAsync(context, search, args, cancellationToken);

                // enter the invocation logic when a match is succesful.
                if (match.Success)
                {
                    var result = await RunAsync(context, match, cancellationToken);

                    await _resultHandle.TryHandleAsync(context, result, Services);

                    return;
                }

                context.TrySetFallback(match);
            }

            // if no searches were found, we send searchfailure.
            if (c is 0)
            {
                await _resultHandle.TryHandleAsync(context, new SearchResult(new SearchException("No commands were found with the provided input.")), Services);
            }

            // if there is a fallback present, we send matchfailure.
            if (context.TryGetFallback(out var fallback))
            {
                await _resultHandle.TryHandleAsync(context, fallback, Services);
            }
        }
        #endregion

        #region Matching
        private async ValueTask<MatchResult> MatchAsync(ICommandContext context, SearchResult search, object[] args, CancellationToken cancellationToken)
        {
            // check command preconditions.
            var check = await CheckAsync(context, search.Command, cancellationToken);

            // verify check success, if not, return the failure.
            if (!check.Success)
                return new(search.Command, new MatchException("Command failed to reach execution state. View inner exception for more details.", check.Exception));

            // read the command parameters in right order.
            var readResult = await ReadAsync(context, search, args, cancellationToken);

            // exchange the reads for result, verifying successes in the process.
            var reads = new object[readResult.Length];
            for (int i = 0; i < readResult.Length; i++)
            {
                // check for read success.
                if (!readResult[i].Success)
                    return new(search.Command, readResult[i].Exception);

                reads[i] = readResult[i];
            }

            // return successful match if execution reaches here.
            return new(search.Command, reads);
        }
        #endregion

        #region Reading
        private async ValueTask<ReadResult[]> ReadAsync(ICommandContext context, SearchResult search, object[] args, CancellationToken cancellationToken)
        {
            context.LogDebug("Attempting argument conversion for {}", search.Command);

            // skip if no parameters exist.
            if (!search.Command.HasParameters)
                return [];

            // determine height of search to discover command name.
            var length = args.Length - search.SearchHeight;

            // check if input equals command length.
            if (search.Command.MaxLength == length)
                return await search.Command.Parameters.RecursiveReadAsync(context, args[length..], 0, cancellationToken);

            // check if input is longer than command, but remainder to concatenate.
            if (search.Command.MaxLength <= length && search.Command.HasRemainder)
                return await search.Command.Parameters.RecursiveReadAsync(context, args[length..], 0, cancellationToken);

            // check if input is shorter than command, but optional parameters to replace.
            if (search.Command.MaxLength > length && search.Command.MinLength <= length)
                return await search.Command.Parameters.RecursiveReadAsync(context, args[length..], 0, cancellationToken);

            // input is too long or too short.
            return [];
        }
        #endregion

        #region Checking
        private async ValueTask<CheckResult> CheckAsync(ICommandContext context, CommandInfo command, CancellationToken cancellationToken)
        {
            context.LogDebug("Attempting validations for {}", command);

            foreach (var precon in command.Preconditions)
            {
                var result = await precon.EvaluateAsync(context, command, cancellationToken);

                if (!result.Success)
                    return result;
            }
            return new();
        }
        #endregion

        #region Running
        private async ValueTask<RunResult> RunAsync(ICommandContext context, MatchResult match, CancellationToken cancellationToken)
        {
            try
            {
                context.LogInformation("Executing {} with {} resolved arguments.", match.Command, match.Reads.Length);

                var module = Services.GetService(match.Command.Module.Type) as ModuleBase;

                module.Context = context;
                module.Command = match.Command;
                module.Services = Services;

                await module.BeforeExecuteAsync(cancellationToken);

                var value = match.Command.Target.Invoke(module, match.Reads);

                await module.AfterExecuteAsync(cancellationToken);

                return module.ReturnTypeResolve(value);
            }
            catch (Exception exception)
            {
                return new(match.Command, exception);
            }
        }
        #endregion

        #region Building
        private IEnumerable<ModuleInfo> BuildComponents(CommandConfiguration configuration)
        {
            var typeReaders = TypeReader.CreateDefaultReaders().UnionBy(TypeReaders, x => x.Type).ToDictionary(x => x.Type, x => x);

            var rootType = typeof(ModuleBase);
            foreach (var assembly in configuration.Assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (rootType.IsAssignableFrom(type)
                        && !type.IsAbstract
                        && !type.ContainsGenericParameters)
                    {
                        yield return new ModuleInfo(type, typeReaders);
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {

        }

        public override string ToString()
        {
            
        }
    }
}
