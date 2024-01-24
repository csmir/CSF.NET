namespace CSF
{
    internal class EnumTypeReader(Type targetEnumType) : TypeReader
    {
        public override Type Type { get; } = targetEnumType;

        public override ValueTask<ReadResult> EvaluateAsync(ICommandContext context, IParameterComponent parameter, string value)
        {
            if (Enum.TryParse(Type, value, true, out var result))
                return ValueTask.FromResult(Success(result));

            return ValueTask.FromResult(Error($"The provided value is not a part the enum specified. Expected: '{Type.Name}', got: '{value}'. At: '{parameter.Name}'"));
        }
    }
}
