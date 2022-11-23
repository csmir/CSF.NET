using CSF.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CSF
{
    public partial class CommandFramework
    {
        /// <summary>
        ///     Runs all execution steps and executes the targetted command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the pipeline order. 
        ///     Not providing all existing steps while overriding this method will cause unpredictable behavior.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="IResult"/> of the execution.</returns>
        protected virtual async Task<IResult> RunPipelineAsync<T>(T context, IServiceProvider provider)
            where T : IContext
        {
            Logger.WriteDebug($"Starting command pipeline for name: '{context.Name}'");

            var searchResult = await SearchAsync(context);
            if (!searchResult.IsSuccess)
                return searchResult;

            var command = searchResult.Match;

            var preconResult = await CheckPreconditionsAsync(context, command, provider);
            if (!preconResult.IsSuccess)
                return preconResult;

            var constructResult = await ConstructAsync(context, command, provider);
            if (!constructResult.IsSuccess)
                return constructResult;

            var readResult = await ParseAsync(context, command, provider);
            if (!readResult.IsSuccess)
                return readResult;

            return await ExecuteAsync(
                context: context,
                command: command,
                commandBase: (ModuleBase<T>)constructResult.Result,
                parameters: ((ParseResult)readResult).Result.ToArray());
        }

        /// <summary>
        ///     Searches through the <see cref="CommandMap"/> for the best possible match.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to change the search flow.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding a <see cref="SearchResult"/> with the returned result.</returns>
        protected virtual async Task<SearchResult> SearchAsync<T>(T context)
            where T : IContext
        {
            await Task.CompletedTask;

            var matches = CommandMap
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var groups = matches.SelectWhere<Module>()
                .OrderBy(x => x.Components.Count)
                .ToList();

            var commands = matches.SelectWhere<Command>()
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

                    var result = await SearchModuleAsync(context, groups[0]);

                    if (result.IsSuccess)
                        return result;

                    context.Name = oldName;
                    context.Parameters = oldParam;
                }
            }

            if (commands.Any())
                return await SearchCommandsAsync(context, commands);

            return CommandNotFoundResult(context);
        }

        /// <summary>
        ///     Searches a module for the best fitting command.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to change the search flow.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module to search in.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding a <see cref="SearchResult"/> with the returned result.</returns>
        protected virtual async Task<SearchResult> SearchModuleAsync<T>(T context, Module module)
            where T : IContext
        {
            var matches = module.Components
                .Where(command => command.Aliases.Any(alias => string.Equals(alias, context.Name, StringComparison.OrdinalIgnoreCase)));

            var commands = matches.SelectWhere<Command>()
                .OrderBy(x => x.Parameters.Count)
                .ToList();

            if (commands.Count < 1)
            {
                var groups = matches.SelectWhere<Module>()
                    .OrderBy(x => x.Components.Count)
                    .ToList();

                if (groups.Any())
                {
                    context.Name = context.Parameters[0].ToString();
                    context.Parameters = context.Parameters.GetRange(1);

                    return await SearchModuleAsync(context, groups[0]);
                }

                else
                    return CommandNotFoundResult(context);
            }

            return await SearchCommandsAsync(context, commands);
        }

        /// <summary>
        ///     Searches a range of commands in the provided group for the best match.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to change the search flow.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="commands">The commands to seek through.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding a <see cref="SearchResult"/> with the returned result.</returns>
        protected virtual async Task<SearchResult> SearchCommandsAsync<T>(T context, IEnumerable<Command> commands)
            where T : IContext
        {
            await Task.CompletedTask;

            Command match = null;
            Command errormatch = null;
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
                    return NoApplicableOverloadResult(context);
                else
                    return SearchResult.FromSuccess(errormatch);
            }

            Logger.WriteTrace($"Found matching command for name: '{context.Name}': {match.Name}");

            return SearchResult.FromSuccess(match);
        }

        /// <summary>
        ///     Returns the error message when the context input was not found.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult CommandNotFoundResult<T>(T context)
            where T : IContext
        {
            return SearchResult.FromError($"Failed to find command with name: {context.Name}.");
        }

        /// <summary>
        ///     Returns the error message when no best match was found for the command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="SearchResult"/> holding the returned error.</returns>
        protected virtual SearchResult NoApplicableOverloadResult<T>(T context)
            where T : IContext
        {
            return SearchResult.FromError($"Failed to find overload that best matches input: {context.Name}.");
        }

        /// <summary>
        ///     Runs all preconditions found on the command and its module and returns the result.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how checks are done and what should be returned if certain checks fail.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="PreconditionResult"/> of the first failed check or success if all checks succeeded.</returns>
        protected virtual async Task<PreconditionResult> CheckPreconditionsAsync<T>(T context, Command command, IServiceProvider provider)
            where T : IContext
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

        /// <summary>
        ///     Creates a new instance of the module the command needs to execute and injects its services.
        /// </summary>
        /// <remarks>
        ///     Can be overridden to modify how the command module is injected and constructed.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ConstructionResult"/> of the built module.</returns>
        protected virtual async Task<ConstructionResult> ConstructAsync<T>(T context, Command command, IServiceProvider provider)
            where T : IContext
        {
            await Task.CompletedTask;

            var services = new List<object>();
            foreach (var dependency in command.Module.Constructor.Dependencies)
            {
                if (dependency.Type == typeof(IServiceProvider))
                    services.Add(provider);
                else
                {
                    var t = provider.GetService(dependency.Type);

                    if (t is null && !dependency.Flags.HasFlag(ParameterFlags.IsNullable))
                        return ServiceNotFoundResult(context, dependency);

                    services.Add(t);
                }
            }

            var obj = command.Module.Constructor.EntryPoint.Invoke(services.ToArray());

            if (!(obj is ModuleBase<T> commandBase))
                return InvalidModuleTypeResult(context, command.Module);

            commandBase.SetContext(context);
            commandBase.SetInformation(command);

            commandBase.SetLogger(Logger);
            commandBase.SetSource(this);

            Logger.WriteTrace($"Succesfully constructed module for {command.Name}");

            return ConstructionResult.FromSuccess(commandBase);
        }

        /// <summary>
        ///     Returns the error message when a service to inject into the module has not been found in the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="dependency">Information about the service to inject.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult ServiceNotFoundResult<T>(T context, Dependency dependency)
            where T : IContext
        {
            return ConstructionResult.FromError($"The service of type {dependency.Type.FullName} does not exist in the current {nameof(IServiceProvider)}.");
        }

        /// <summary>
        ///     Returns the error message when the module in question cannot be cast to an <see cref="IModuleBase"/>.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="module">The module that failed to cast to an <see cref="IModuleBase"/>.</param>
        /// <returns>A <see cref="ConstructionResult"/> holding the returned error.</returns>
        protected virtual ConstructionResult InvalidModuleTypeResult<T>(T context, Module module)
            where T : IContext
        {
            return ConstructionResult.FromError($"Failed to interpret module of type {module.Type.FullName} with type of {nameof(ModuleBase<T>)}");
        }

        /// <summary>
        ///     Tries to parse all parameter inputs provided by the <see cref="IContext"/> to its expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to parameter parsing or catch certain attributes being present.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> used to populate modules. If null, non-nullable services to inject will throw.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ParseResult"/> of the parsed parameters of this command.</returns>
        protected virtual async Task<IResult> ParseAsync<T>(T context, Command command, IServiceProvider provider)
            where T : IContext
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
                    var missingResult = await ResolveMissingValue(context, parameter);

                    if (!missingResult.IsSuccess)
                        return OptionalValueNotPopulated(context);

                    var resultType = missingResult.Result.GetType();

                    if (resultType == parameter.Type || missingResult.Result == Type.Missing)
                    {
                        parameters.Add(missingResult.Result);
                        continue;
                    }
                    else
                        return MissingOptionalFailedMatch(context, parameter.Type, resultType);
                }

                if (parameter.Type == typeof(string) || parameter.Type == typeof(object))
                {
                    parameters.Add(context.Parameters[index]);
                    index++;
                    continue;
                }

                var result = await parameter.TypeReader.ReadAsync(context, parameter, context.Parameters[index], provider);

                if (!result.IsSuccess)
                    return result;

                parameters.Add(result.Result);
                index++;
            }

            Logger.WriteTrace($"Succesfully populated parameters for {command.Name}. Count: {parameters.Count}");

            return ParseResult.FromSuccess(parameters.ToArray());
        }

        /// <summary>
        ///     Called when an optional parameter has a lacking value.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add uses for <see cref="SelfIfNullAttribute"/>'s.
        ///     <br/>
        ///     The result will fail to resolve and exit execution if the type does not match the provided <see cref="Parameter.Type"/> or <see cref="Type.Missing"/>.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="param"></param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="TypeReaderResult"/> for the target parameter.</returns>
        protected virtual Task<TypeReaderResult> ResolveMissingValue<T>(T context, Parameter param)
            where T : IContext
        {
            return Task.FromResult(TypeReaderResult.FromSuccess(Type.Missing));
        }

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, Parameter)"/> returned a type that did not match the expected type.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="expectedType">The type that was expected to return.</param>
        /// <param name="returnedType">The returned type.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        protected virtual ParseResult MissingOptionalFailedMatch<T>(T context, Type expectedType, Type returnedType)
            where T : IContext
        {
            return ParseResult.FromError($"Returned type does not match expected result. Expected: '{expectedType.Name}'. Got: '{returnedType.Name}'");
        }

        /// <summary>
        ///     Returns the error when <see cref="ResolveMissingValue{T}(T, Parameter)"/> failed to return a valid result. 
        ///     This method has to return <see cref="Type.Missing"/> if no self-implemented value has been returned.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <returns>A <see cref="ParseResult"/> holding the returned error.</returns>
        protected virtual ParseResult OptionalValueNotPopulated<T>(T context)
            where T : IContext
        {
            return ParseResult.FromError($"Optional parameter did not get {nameof(Type.Missing)} or self-populated value.");
        }

        /// <summary>
        ///     Tries to execute the provided command.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to add extra steps to the execution of the command with all prior steps completed.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="commandBase">The module needed to execute the command method.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="parameters">The parsed parameters required to populate the command method.</param>
        /// <returns>An asynchronous <see cref="Task"/> holding the <see cref="ExecuteResult"/> of this executed command.</returns>
        protected virtual async Task<IResult> ExecuteAsync<T>(T context, Command command, ModuleBase<T> commandBase, object[] parameters)
            where T : IContext
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

                        var unhandledResult = ProcessUnhandledReturnType(context, returnValue);
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
                return UnhandledExceptionResult(context, command, ex);
            }
        }

        /// <summary>
        ///     Returns the error message when an unhandled return type has been returned from the command method.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="returnValue">The returned value of the method.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        protected virtual ExecuteResult ProcessUnhandledReturnType<T>(T context, object returnValue)
            where T : IContext
        {
            return ExecuteResult.FromError($"Received an unhandled type from method execution: {returnValue.GetType().Name}. \n\rConsider overloading {nameof(ProcessUnhandledReturnType)} if this is intended.");
        }

        /// <summary>
        ///     Returns the error message when command execution fails on the user's end.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify the error response.
        /// </remarks>
        /// <typeparam name="T">The <see cref="IContext"/> used to run the command.</typeparam>
        /// <param name="context">The <see cref="IContext"/> used to run the command.</param>
        /// <param name="command">Information about the command that's being executed.</param>
        /// <param name="ex">The exception that occurred while executing the command.</param>
        /// <returns>An <see cref="ExecuteResult"/> holding the returned error.</returns>
        protected virtual ExecuteResult UnhandledExceptionResult<T>(T context, Command command, Exception ex)
            where T : IContext
        {
            return ExecuteResult.FromError(ex.Message, ex);
        }
    }
}
