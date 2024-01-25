using CSF.Reflection;

namespace CSF.TypeReaders
{
    internal class EnumTypeReader(Type targetEnumType) : TypeReader
    {
        private static readonly Dictionary<Type, EnumTypeReader> _readers = [];

        public override Type Type { get; } = targetEnumType;

        public override ValueTask<ReadResult> EvaluateAsync(ICommandContext context, IArgument parameter, string value)
        {
            if (Enum.TryParse(Type, value, true, out var result))
                return ValueTask.FromResult(Success(result));

            return ValueTask.FromResult(Error($"The provided value is not a part the enum specified. Expected: '{Type.Name}', got: '{value}'. At: '{parameter.Name}'"));
        }

        internal static EnumTypeReader GetOrCreate(Type type)
        {
            if (_readers.TryGetValue(type, out var reader))
                return reader;

            _readers.Add(type, reader = new EnumTypeReader(type));

            return reader;
        }
    }
}
