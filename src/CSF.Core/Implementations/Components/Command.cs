using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents the information required to execute commands.
    /// </summary>
    public sealed class Command : IConditionalComponent, IParameterContainer
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<Attribute> Attributes { get; }

        /// <inheritdoc/>
        public IList<IPrecondition> Preconditions { get; }

        /// <inheritdoc/>
        public IList<IParameterComponent> Parameters { get; }

        /// <inheritdoc/>
        public int MinLength { get; }

        /// <inheritdoc/>
        public int MaxLength { get; }

        /// <summary>
        ///     The command aliases.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        ///     The command module.
        /// </summary>
        public Module Module { get; }

        /// <summary>
        ///     The command method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        ///     Represents if the command is an error overload.
        /// </summary>
        public bool IsErrorOverload { get; }

        /// <summary>
        ///     Creates a new <see cref="Command"/>.
        /// </summary>
        /// <param name="module">The root module of this command.</param>
        /// <param name="method">The method information of this command.</param>
        /// <param name="aliases">The aliases of this command.</param>
        public Command(Module module, MethodInfo method, string[] aliases)
        {
            Method = method;
            Module = module;

            Attributes = module.Attributes.Concat(GetAttributes())
                .ToList();
            Preconditions = module.Preconditions.Concat(GetPreconditions())
                .ToList();
            Parameters = GetParameters()
                .ToList();

            var remainderParameters = Parameters.Where(x => x.Flags.HasFlag(ParameterFlags.Remainder));
            if (remainderParameters.Any())
            {
                if (remainderParameters.Count() > 1)
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} cannot exist on multiple parameters at once.");

                if (!Parameters.Last().Flags.HasFlag(ParameterFlags.Remainder))
                    throw new InvalidOperationException($"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");
            }

            if (Attributes.Any(x => x is ErrorOverloadAttribute))
            {
                if (Parameters.Any())
                    throw new InvalidOperationException($"{nameof(ErrorOverloadAttribute)} cannot exist on a method with parameters.");

                IsErrorOverload = true;
            }

            Name = aliases[0];
            Aliases = aliases;

            (int min, int max) = GetLength();

            MinLength = min;
            MaxLength = max;
        }

        /// <summary>
        ///     Checks the preconditions of this command and returns the result.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="services">The services used to execute the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public async ValueTask<PreconditionResult> CheckAsync<T>(T context, IServiceProvider services, CancellationToken cancellationToken)
            where T : IContext
        {
            foreach (var precon in Preconditions)
            {
                var result = await precon.CheckAsync(context, this, services, cancellationToken);

                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        /// <summary>
        ///     Reads the parameters of this command and returns the result.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="typeReaders">The typereaders used to read the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public async ValueTask<IResult> ReadAsync<T>(T context, TypeReaderContainer typeReaders, CancellationToken cancellationToken)
            where T : IContext
        {
            var result = await ReadAsync(context, 0, typeReaders, cancellationToken);

            if (!result.IsSuccess)
                return result;

            return ParseResult.FromSuccess(((ParseResult)result).Result, -1);
        }

        /// <summary>
        ///     Reads the containers of the parameter and returns the result.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="index">The index used to increment reader position.</param>
        /// <param name="typeReaders">The typereaders used to read the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public async ValueTask<IResult> ReadAsync<T>(T context, int index, TypeReaderContainer typeReaders, CancellationToken cancellationToken)
            where T : IContext
        {
            var parameters = new List<object>();

            foreach (var parameter in Parameters)
            {
                if (parameter.Flags.HasRemainder())
                {
                    parameters.Add(string.Join(" ", context.Parameters.Skip(index)));
                    break;
                }

                if (parameter.Flags.HasOptional() && context.Parameters.Count <= index)
                {
                    parameters.Add(Type.Missing);
                    continue;
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
                    var result = await ReadAsync(context, index, typeReaders, cancellationToken);

                    if (!result.IsSuccess)
                        return result;

                    if (result is ParseResult argsResult)
                    {
                        index = argsResult.Placement;
                        parameters.Add(complexParam.Constructor.EntryPoint.Invoke(argsResult.Result.ToArray()));
                    }
                }

                else if (parameter is BaseParameter normal)
                {
                    var result = await typeReaders.Values[normal.Type].ReadAsync(context, normal, context.Parameters[index], cancellationToken);

                    if (!result.IsSuccess)
                        return result;

                    index++;
                    parameters.Add(result.Result);
                }
            }

            return ParseResult.FromSuccess(parameters, index);
        }

        /// <summary>
        ///     Fetches the execution context and executes the command logic.
        /// </summary>
        /// <typeparam name="T">The context containing information necessary to search and match the command.</typeparam>
        /// <param name="context">The context containing information necessary to search and match the command.</param>
        /// <param name="parameters">The parameters to populate the method with.</param>
        /// <param name="services">The services used to execute the command.</param>
        /// <param name="cancellationToken">Token to be used to signal the code to break.</param>
        /// <returns>An asynchronous <see cref="ValueTask"/> holding the result of the executed command.</returns>
        public async ValueTask<IResult> ExecuteAsync<T>(T context, IEnumerable<object> parameters, IServiceProvider services, CancellationToken cancellationToken)
            where T : IContext
        {
            try
            {
                var module = services.GetService(Module.Type) as ModuleBase;

                module.SetContext(context);
                module.SetCommand(this);

                await module.BeforeExecuteAsync(cancellationToken);

                var returnValue = Method.Invoke(module, parameters.ToArray());

                await module.AfterExecuteAsync(cancellationToken);

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
                        throw new NotSupportedException("Specified return type is not supported.");
                }

                return result;
            }
            catch (Exception ex)
            {
                return ExecuteResult.FromError(ex.Message, ex);
            }
        }

        private (int, int) GetLength()
        {
            var minLength = 0;
            var maxLength = 0;

            foreach (var parameter in Parameters)
            {
                if (parameter is ComplexParameter complexParam)
                {
                    maxLength += complexParam.MaxLength;
                    minLength += complexParam.MinLength;
                }

                if (parameter is BaseParameter defaultParam)
                {
                    maxLength++;
                    if (!defaultParam.Flags.HasFlag(ParameterFlags.Optional))
                        minLength++;
                }
            }


            return (minLength, maxLength);
        }

        private IEnumerable<IParameterComponent> GetParameters()
        {
            foreach (var parameter in Method.GetParameters())
            {
                if (parameter.GetCustomAttributes().Any(x => x is ComplexAttribute))
                    yield return new ComplexParameter(parameter);
                else
                    yield return new BaseParameter(parameter);
            }
        }

        private IEnumerable<PreconditionAttribute> GetPreconditions()
        {
            foreach (var attr in Attributes)
                if (attr is PreconditionAttribute precondition)
                    yield return precondition;
        }

        private IEnumerable<Attribute> GetAttributes()
        {
            foreach (var attribute in Method.GetCustomAttributes(true))
                if (attribute is Attribute attr)
                    yield return attr;
        }

        /// <summary>
        ///     Formats the type into a readable signature.
        /// </summary>
        /// <returns>A string containing a readable signature.</returns>
        public override string ToString()
            => $"{Module}.{Method.Name}['{Name}']({string.Join(", ", Parameters)})";
    }
}
