using CSF.Core;
using CSF.Reflection;
using System.ComponentModel;

namespace CSF.Helpers
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class ExecutionHelpers
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<SearchResult> RecursiveSearch(this IEnumerable<IConditional> components, object[] args, int searchHeight)
        {
            List<SearchResult> discovered = [];

            // select command by name or alias.
            var selection = components.Where(command => command.Aliases.Any(x => x == (string)args[searchHeight]));

            foreach (var component in selection)
            {
                if (component is ModuleInfo module)
                {
                    // add the cluster found in the next iteration, if any.
                    var nested = module.Components.RecursiveSearch(args, searchHeight + 1);
                    discovered.AddRange(nested);
                }
                else
                    // add the top level matches immediately.
                    discovered.Add(new(component as CommandInfo, searchHeight));

                // when the ranges fail, no results should return.
            }

            return discovered;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static async Task<ReadResult[]> RecursiveReadAsync(this IArgument[] param, ICommandContext context, object[] args, int index, CancellationToken cancellationToken)
        {
            static async ValueTask<ReadResult> ReadAsync(IArgument param, ICommandContext context, object arg, CancellationToken cancellationToken)
            {
                if (arg.GetType() == param.Type)
                    return new(arg);

                if (param.IsNullable && arg is null or "null" or "nothing")
                    return new(arg);

                return await param.TypeReader.ObjectEvaluateAsync(context, param, arg, cancellationToken);
            }

            var results = new ReadResult[param.Length];

            for (int i = 0; i < param.Length; i++)
            {
                var parameter = param[i];

                if (parameter.IsRemainder)
                {
                    var input = string.Join(" ", args.Skip(index));
                    if (parameter.Type == typeof(string))
                        results[i] = new(input);
                    else
                        results[i] = await ReadAsync(parameter, context, input, cancellationToken);

                    break;
                }

                if (parameter.IsOptional && args.Length <= index)
                {
                    results[i] = new(Type.Missing);
                    continue;
                }

                if (parameter is ComplexArgumentInfo complex)
                {
                    var result = await complex.Parameters.RecursiveReadAsync(context, args, index, cancellationToken);

                    index += result.Length;

                    if (result.Any(x => !x.Success))
                    {
                        try
                        {
                            var obj = complex.Constructor.Invoke(result.Select(x => x.Value).ToArray());
                            results[i] = new(obj);
                        }
                        catch (Exception ex)
                        {
                            results[i] = new(ex);
                        }
                    }
                    continue;
                }

                results[i] = await ReadAsync(parameter, context, args[index], cancellationToken);
                index++;
            }

            return results;
        }
    }
}
