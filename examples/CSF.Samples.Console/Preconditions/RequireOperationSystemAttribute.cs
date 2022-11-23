namespace CSF.Console
{
    // This precondition will check if the operating system executing the command matches the provided platform.
    public sealed class RequireOperatingSystemAttribute : PreconditionAttribute
    {
        public PlatformID Platform { get; }

        public RequireOperatingSystemAttribute(PlatformID platform)
        {
            Platform = platform;
        }

        public override Task<PreconditionResult> CheckAsync(IContext context, Command info, IServiceProvider provider)
        {
            if (Environment.OSVersion.Platform != Platform)
                return Task.FromResult(PreconditionResult.FromError("Current OS platform does not match the expected platform!"));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
