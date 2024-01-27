using CSF.Helpers;
using CSF.Preconditions;
using CSF.TypeReaders;
using System.Reflection;

namespace CSF.Reflection
{

    public sealed class CommandInfo : IConditional, IArgumentBucket
    {

        public string Name { get; }


        public Attribute[] Attributes { get; }


        public PreconditionAttribute[] Preconditions { get; }


        public bool HasPreconditions { get; }


        public IArgument[] Parameters { get; }


        public bool HasParameters { get; }


        public bool HasRemainder { get; }


        public int MinLength { get; }


        public int MaxLength { get; }


        public string[] Aliases { get; }


        public byte Priority { get; }


        public ModuleInfo Module { get; }


        public MethodInfo Target { get; }

        internal CommandInfo(ModuleInfo module, MethodInfo method, string[] aliases, IDictionary<Type, TypeReader> typeReaders)
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

            Priority = attributes.SelectFirstOrDefault<PriorityAttribute>()?.Priority ?? 0;

            Target = method;
            Module = module;

            Attributes = attributes;
            Preconditions = preconditions;
            HasPreconditions = preconditions.Length > 0;

            Parameters = parameters;
            HasParameters = parameters.Length > 0;
            HasRemainder = parameters.Any(x => x.IsRemainder);

            Name = aliases[0];
            Aliases = aliases;

            MinLength = minLength;
            MaxLength = maxLength;
        }


        public override string ToString()
            => $"{Module}.{Target.Name}['{Name}']({string.Join<IArgument>(", ", Parameters)})";
    }
}
