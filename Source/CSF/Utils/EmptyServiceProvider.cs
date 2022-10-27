using System;

namespace CSF
{
    internal sealed class EmptyServiceProvider : IServiceProvider
    {
        public static EmptyServiceProvider Instance { get; } = new EmptyServiceProvider();

        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
