using Microsoft.Extensions.Hosting;

namespace CSF.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder WithCommandFramework(this IHostBuilder hostBuilder, Action<HostBuilderContext, FrameworkBuilderContext> action = null)
        {
            hostBuilder.WithCommandFramework<CommandFramework>(action);

            return hostBuilder;
        }

        public static IHostBuilder WithCommandFramework<T>(this IHostBuilder hostBuilder, Action<HostBuilderContext, FrameworkBuilderContext> action = null)
            where T : CommandFramework
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var fxContext = new FrameworkBuilderContext();

                action?.Invoke(hostContext, fxContext);

                services.AddCommandFramework<T>(fxContext);
            });

            return hostBuilder;
        }
    }
}
