namespace CSF
{
    internal static class CommandManagerHelper
    {
        public static IEnumerable<SearchResult> RecursiveSearch(this IEnumerable<IConditionalComponent> components, object[] args, int searchHeight)
        {
            List<SearchResult> discovered = [];

            // select command by name or alias.
            var selection = components.Where(command => command.Aliases.Any(x => x == (string)args[searchHeight]));

            foreach (var component in selection)
            {
                if (component is Module module)
                {
                    // add the cluster found in the next iteration, if any.
                    var nested = module.Components.RecursiveSearch(args, searchHeight + 1);
                    discovered.AddRange(nested);
                }
                else
                    // add the top level matches immediately.
                    discovered.Add(new(component as Command, searchHeight));

                // when the ranges fail, no results should return.
            }

            return discovered;
        }

        public static async Task<ReadResult[]> RecursiveReadAsync(this IParameterComponent[] param, ICommandContext context, object[] args, int index)
        {

            static async ValueTask<ReadResult> ReadAsync(IParameterComponent param, ICommandContext context, object arg)
            {
                if (arg.GetType() == param.Type)
                    return new(arg);

                if (param.IsNullable && arg is null or "null" or "nothing")
                    return new(arg);

                return await param.TypeReader.ObjectEvaluateAsync(context, param, arg);
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
                        results[i] = await ReadAsync(parameter, context, input);

                    break;
                }

                if (parameter.IsOptional && args.Length <= index)
                {
                    results[i] = new(Type.Missing);
                    continue;
                }

                if (parameter is ComplexParameter complex)
                {
                    var result = await complex.Parameters.RecursiveReadAsync(context, args, index);

                    index += result.Length;

                    if (result.Any(x => !x.Success))
                    {
                        try
                        {
                            var obj = complex.Constructor.Target.Invoke(result.Select(x => x.Value).ToArray());
                            results[i] = new(obj);
                        }
                        catch (Exception ex)
                        {
                            results[i] = new(ex);
                        }
                    }
                    continue;
                }

                results[i] = await ReadAsync(parameter, context, args[index]);
                index++;
            }

            return results;
        }
    }
}
