[assembly: CLSCompliant(true)]

namespace CSF
{
    /// <summary>
    ///     The root type of the Command Standardization Framework (CSF). This type is responsible for setting up the execution pipeline, handling command input and managing modules.
    /// </summary>
    /// <remarks>
    ///     Guides and documentation can be found at: <see href="https://github.com/csmir/CSF.NET/wiki"/>
    /// </remarks>
    public class CommandManager
    {
        /// <summary>
        ///     Gets the components registered to this manager.
        /// </summary>
        public HashSet<IConditionalComponent> Components { get; }

        public async Task<IResult> ExecuteAsync(ICommandContext context, object[] args)
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
                return new SearchResult(new SearchException(""));

            return fallback;
        }

        public IEnumerable<SearchResult> Search(object[] args)
        {
            // recursively search for commands in the execution.
            return Components.RecursiveSearch(args, 0);
        }

        public static async Task<MatchResult> MatchAsync(ICommandContext context, SearchResult search, object[] args)
        {
            // check command preconditions.
            var check = await CheckAsync(context, search.Command);

            // verify check success, if not, return the failure.
            if (!check.Success)
                return new(search.Command, new MatchException("", check.Exception));

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

        public static async Task<ReadResult[]> ReadAsync(ICommandContext context, SearchResult search, object[] args)
        {
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
        
        public static async Task<CheckResult> CheckAsync(ICommandContext context, Command command)
        {
            foreach (var precon in command.Preconditions)
            {
                if (!await precon.EvaluateAsync(context, command))
                    return new(new CheckException(""));
            }
            return new();
        }

        public static async Task<RunResult> RunAsync(ICommandContext context, MatchResult match)
        {
            try
            {
                var module = context.Options.Scope.ServiceProvider.GetService(match.Command.Module.Type) as ModuleBase;

                module.Context = context;
                module.Command = match.Command;

                await module.BeforeExecuteAsync();

                var value = match.Command.Target.Invoke(module, match.Reads);

                await module.AfterExecuteAsync();

                return module.ReturnTypeHandle(value);
            }
            catch (Exception exception)
            {
                return new(match.Command, exception);
            }
        }
    }
}
