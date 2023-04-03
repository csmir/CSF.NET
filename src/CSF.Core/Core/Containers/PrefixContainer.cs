using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public sealed class PrefixContainer
    {
        public List<IPrefix> Values { get; }

        public PrefixContainer(IEnumerable<IPrefix> prefixes)
        {
            Values = prefixes.ToList();
        }

        public PrefixContainer Include(IPrefix prefix)
        {
            if (!Values.Contains(prefix))
                Values.Add(prefix);

            return this;
        }

        public PrefixContainer Exclude(IPrefix prefix)
        {
            Values.Remove(prefix);
            return this;
        }
    }
}
