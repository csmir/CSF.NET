﻿using CSF.Core;
using CSF.Helpers;
using CSF.Preconditions;
using CSF.TypeConverters;
using System.Reflection;

namespace CSF.Reflection
{
    /// <summary>
    ///     Reveals information about a command.
    /// </summary>
    public sealed class CommandInfo : IConditional, IArgumentBucket
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Attribute[] Attributes { get; }

        /// <inheritdoc />
        public PreconditionAttribute[] Preconditions { get; }

        /// <inheritdoc />
        public bool HasPreconditions { get; }

        /// <inheritdoc />
        public IArgument[] Arguments { get; }

        /// <inheritdoc />
        public bool HasArguments { get; }

        /// <inheritdoc />
        public bool HasRemainder { get; }

        /// <inheritdoc />
        public int MinLength { get; }

        /// <inheritdoc />
        public int MaxLength { get; }

        /// <inheritdoc />
        public string[] Aliases { get; }

        /// <summary>
        ///     Gets the priority of this command.
        /// </summary>
        public byte Priority { get; }

        /// <summary>
        ///     Gets the module in which the command is known.
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        ///     Gets the invocation target of this command.
        /// </summary>
        public MethodInfo Target { get; }

        internal CommandInfo(ModuleInfo module, MethodInfo method, string[] aliases, IDictionary<Type, TypeConverter> typeReaders)
        {
            var attributes = method.GetAttributes(true);
            var preconditions = attributes.GetPreconditions();
            var parameters = method.GetParameters(typeReaders);

            var (minLength, maxLength) = parameters.GetLength();

            if (parameters.Any(x => x.Attributes.Contains<RemainderAttribute>(false)))
            {
                if (parameters[^1].IsRemainder)
                {
                    ThrowHelpers.InvalidOp($"{nameof(RemainderAttribute)} can only exist on the last parameter of a method.");
                }
            }

            if (method.ReturnType == typeof(Task))
            {

            }

            Priority = attributes.SelectFirstOrDefault<PriorityAttribute>()?.Priority ?? 0;

            Target = method;
            Module = module;

            Attributes = attributes;
            Preconditions = preconditions;
            HasPreconditions = preconditions.Length > 0;

            Arguments = parameters;
            HasArguments = parameters.Length > 0;
            HasRemainder = parameters.Any(x => x.IsRemainder);

            Name = aliases[0];
            Aliases = aliases;

            MinLength = minLength;
            MaxLength = maxLength;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{Module}.{Target.Name}['{Name}']({string.Join<IArgument>(", ", Arguments)})";
    }
}
