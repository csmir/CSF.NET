namespace CSF.Tests
{
    public class TimeOnlyTypeReader : TypeReader<TimeOnly>
    {
        public override Result Evaluate(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            return Success(TimeOnly.MinValue);
        }
    }
}
