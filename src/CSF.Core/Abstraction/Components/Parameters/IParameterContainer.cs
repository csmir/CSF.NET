using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    /// <summary>
    ///     Represents a container that holds and handles parameters.
    /// </summary>
    public interface IParameterContainer
    {
        /// <summary>
        ///     The list of parameters for this component.
        /// </summary>
        public IList<IParameterComponent> Parameters { get; }

        /// <summary>
        ///     The minimum required length to use a command.
        /// </summary>
        public int MinLength { get; }

        /// <summary>
        ///     The optimal length to use a command. If remainder is specified, the count will be set to infinity.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        ///     Attempts to parse a container into a result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="index">The index used to increment reader position.</param>
        /// <param name="typeReaders">The typereaders used to read the command.</param>
        /// <param name="cancellationToken">Used to signal the command execution to stop.</param>
        /// <returns>An as</returns>
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

            return ParseResult.Success(parameters, index);
        }
    }
}
