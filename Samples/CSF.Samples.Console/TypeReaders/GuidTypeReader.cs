namespace CSF.Console
{
    // This type reader will try to convert the provided string to a GUID before passing it through to the command.
    public sealed class GuidTypeReader : TypeReader<Guid>
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, ParameterInfo info, string value, IServiceProvider provider)
        {
            if (Guid.TryParse(value, out Guid id))
                return Task.FromResult(TypeReaderResult.FromSuccess(id));
            return Task.FromResult(TypeReaderResult.FromError($"Provided value is not a valid GUID. Occurred at: '{info.Name}'"));
        }
    }
}
