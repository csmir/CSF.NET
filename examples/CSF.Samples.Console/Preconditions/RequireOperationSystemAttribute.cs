// Documentation of this file can be found at: https://github.com/csmir/CSF.NET/wiki/Preconditions.

using CSF.Core;
using CSF.Preconditions;
using CSF.Reflection;

namespace CSF.Samples
{
    public class RequireOperatingSystemAttribute(PlatformID platform) : PreconditionAttribute
    {
        public PlatformID Platform { get; } = platform;

        public override ValueTask<CheckResult> EvaluateAsync(ICommandContext context, IServiceProvider services, CommandInfo command, CancellationToken cancellationToken)
        {
            if (Environment.OSVersion.Platform == Platform)
                return ValueTask.FromResult(Success());

            return ValueTask.FromResult(Error("The platform does not support this operation."));
        }
    }
}
