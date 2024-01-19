using CSF;

namespace XProject
{
    public class RequireOperatingSystemAttribute : PreconditionAttribute
    {
        public PlatformID Platform { get; }

        public RequireOperatingSystemAttribute(PlatformID platform)
        {
            Platform = platform;
        }

        public override Result EvaluateAsync(ICommandContext context, Command command, IServiceProvider provider)
        {
            if (Environment.OSVersion.Platform == Platform)
                return Success();

            return Failure("The platform this command was executed does not support this operation.");
        }
    }
}
