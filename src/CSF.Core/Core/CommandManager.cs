using CSF.TypeReaders;
using CSF.Helpers;
using CSF.Reflection;
using CSF.Exceptions;

[assembly: CLSCompliant(true)]

namespace CSF
{
    public class CommandManager
    {
        public IServiceProvider Services { get; }

        public IReadOnlySet<IConditional> Components { get; }

        public TypeReader[] TypeReaders { get; }

        public CommandConfiguration Configuration { get; }

        public CommandManager(IServiceProvider services, CommandConfiguration configuration)
        {
            TypeReaders = configuration.TypeReaders.Distinct().ToArray();

            if (configuration.Assemblies == null || configuration.Assemblies.Length == 0)
            {
                ThrowHelpers.ArgMissing(nameof(configuration.Assemblies));
            }

            Components = BuildComponents(configuration)
                .SelectMany(x => x.Components)
                .ToHashSet();

            Services = services;

            Configuration = configuration;
        }

        public virtual async Task<IResult> ExecuteAsync(ICommandContext context, params object[] args)
        {
            // search all relevant commands.
            var searches = Search(args);

            // define a fallback for unsuccesful execution.
            MatchResult? fallback = default;

            // order searches by descending for priority definitions.
            foreach (var search in searches.OrderByDescending(x => x.Command.Priority))
            {
                var match = await MatchAsync(context, search, args);

                if (fallback is not null)
                    fallback = match;

                if (match.Success)
                    return await RunAsync(context, match);
            }

            if (!fallback.HasValue)
                return new SearchResult(new SearchException("No command was found with the provided input."));

            return fallback;
        }

        public virtual IEnumerable<SearchResult> Search(object[] args)
        {
            // recursively search for commands in the execution.
            return Components.RecursiveSearch(args, 0);
        }

        #region Matching
        private async ValueTask<MatchResult> MatchAsync(ICommandContext context, SearchResult search, object[] args)
        {
            // check command preconditions.
            var check = await CheckAsync(context, search.Command);

            // verify check success, if not, return the failure.
            if (!check.Success)
                return new(search.Command, new MatchException("Command failed to reach execution state. View inner exception for more details.", check.Exception));

            // read the command parameters in right order.
            var readResult = await ReadAsync(context, search, args);

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
        private async ValueTask<ReadResult[]> ReadAsync(ICommandContext context, SearchResult search, object[] args)
        {
            context.LogDebug("Attempting argument conversion for {}", search.Command);

            // skip if no parameters exist.
            if (!search.Command.HasParameters)
                return [];

            // determine height of search to discover command name.
            var length = args.Length - search.SearchHeight;

            // check if input equals command length.
            if (search.Command.MaxLength == length)
                return await search.Command.Parameters.RecursiveReadAsync(context, args[length..], 0);

            // check if input is longer than command, but remainder to concatenate.
            if (search.Command.MaxLength <= length && search.Command.HasRemainder)
                return await search.Command.Parameters.RecursiveReadAsync(context, args[length..], 0);

            // check if input is shorter than command, but optional parameters to replace.
            if (search.Command.MaxLength > length && search.Command.MinLength <= length)
                return await search.Command.Parameters.RecursiveReadAsync(context, args[length..], 0);

            // input is too long or too short.
            return [];
        }
        #endregion

        #region Checking
        private async ValueTask<CheckResult> CheckAsync(ICommandContext context, CommandInfo command)
        {
            context.LogDebug("Attempting validations for {}", command);

            foreach (var precon in command.Preconditions)
            {
                var result = await precon.EvaluateAsync(context, command);

                if (!result.Success)
                    return result;
            }
            return new();
        }
        #endregion

        #region Running
        private async ValueTask<RunResult> RunAsync(ICommandContext context, MatchResult match)
        {
            try
            {
                context.LogInformation("Executing {} with {} resolved arguments.", match.Command, match.Reads.Length);

                var module = Services.GetService(match.Command.Module.Type) as ModuleBase;

                module.Context = context;
                module.Command = match.Command;
                module.Services = Services;

                await module.BeforeExecuteAsync();

                var value = match.Command.Target.Invoke(module, match.Reads);

                await module.AfterExecuteAsync();

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
    }
}
