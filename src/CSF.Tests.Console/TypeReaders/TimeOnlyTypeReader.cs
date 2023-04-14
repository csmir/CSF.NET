namespace CSF.Tests
{
    public class TimeOnlyTypeReader : TypeReader<TimeOnly>
    {
        public override object Read(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            return TimeOnly.MinValue;
        }
    }
}
