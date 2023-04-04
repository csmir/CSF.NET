using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

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

        public async ValueTask<PreconditionResult> CheckAsync<T>(T context, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            foreach (var precon in Preconditions)
            {
                var result = await precon.CheckAsync(context, this, provider, cancellationToken);

                if (!result.IsSuccess)
                    return result;
            }

            return PreconditionResult.FromSuccess();
        }

        public async ValueTask<IResult> ReadAsync<T>(T context, TypeReaderContainer typeReaders, CancellationToken cancellationToken)
            where T : IContext
        {
            var result = await ((IParameterContainer)this).ReadAsync(context, 0, typeReaders, cancellationToken);

            if (!result.IsSuccess)
                return result;

            return ArgsResult.FromSuccess(((ArgsResult)result).Result, -1);
        }

        public async ValueTask<IResult> ExecuteAsync<T>(T context, IEnumerable<object> parameters, IServiceProvider provider, CancellationToken cancellationToken)
            where T : IContext
        {
            try
            {
                var module = provider.GetService(Module.Type) as ModuleBase;

                module.SetContext(context);
                module.SetInformation(this);

                await module.BeforeExecuteAsync(this, cancellationToken);

                var returnValue = Method.Invoke(module, parameters.ToArray());

                await module.AfterExecuteAsync(this, cancellationToken);

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

        public override string ToString()
            => $"{Module}.{Method.Name}['{Name}']({string.Join(", ", Parameters)})";
    }
}
