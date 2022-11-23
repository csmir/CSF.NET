using System.Reflection;

namespace CSF.Hosting
{
    public class CommandHostingContext<T> : ICommandHostingContext
        where T : CommandConfiguration
    {
        public T Configuration { get; set; } = default!;

        public Assembly RegistrationAssembly { get; set; } = Assembly.GetEntryAssembly()!;
    }
}
