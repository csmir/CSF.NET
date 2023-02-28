using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CSF.Results
{
    internal static class ResultContextCache
    {
        private static readonly Lazy<IDictionary<Guid, object>> _cache = new Lazy<IDictionary<Guid, object>>(() =>
        {
            return new Dictionary<Guid, object>();
        }, true);

        public static void DisposeCacheKey(Guid guid)
            => _cache.Value.Remove(guid);
    }
}
