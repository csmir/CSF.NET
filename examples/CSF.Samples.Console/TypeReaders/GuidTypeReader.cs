namespace CSF.Console
{
    // This type reader will try to convert the provided string to a GUID before passing it through to the command.
    public sealed class GuidTypeReader : TypeReader<Guid>
    {
        public override Result Evaluate(ICommandContext context, IParameterComponent parameter, IServiceProvider services, string value)
        {
            if (Guid.TryParse(value, out Guid guid))
                return Success(guid);

            return Failure("Failed to parse the input string as a GUID.");
        }
    }
}
