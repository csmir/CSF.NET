using System;
using System.Threading.Tasks;

namespace CSF
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public sealed class RequirePrefixAttribute : PreconditionAttribute
    {
        public IPrefix[] AllowedPrefixes;

        public RequirePrefixAttribute(params IPrefix[] prefixes)
        {
            AllowedPrefixes = prefixes;
        }

        public override Task<PreconditionResult> CheckAsync(IContext context, Command info, IServiceProvider provider)
        {
            foreach (var prefix in AllowedPrefixes)
            {
                if (context.Parameters[0].ToString().StartsWith(prefix.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Failed to find any allowed prefix with matching type."));
        }
    }
}
