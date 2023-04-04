using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public sealed class PrefixContainer
    {
        public IEnumerable<IPrefix> Values { get; }

        public PrefixContainer(IEnumerable<IPrefix> prefixes)
            => Values = prefixes;
    }

    public static class PrefixContainerExtensions
    {
        public static IServiceCollection AddPrefixes(this IServiceCollection collection, FrameworkBuilderContext context)
        {
            var container = new PrefixContainer(context.Prefixes);

            collection.TryAddSingleton(container);

            return collection;
        }
    }
}
