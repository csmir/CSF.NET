using Microsoft.Extensions.Hosting;

namespace CSF.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder WithCommandFramework(this IHostBuilder hostBuilder, Action<HostBuilderContext, ManagerBuilderContext> action = null)
        {
            hostBuilder.WithCommandFramework<CommandManager>(action);

            return hostBuilder;
        }

        public static IHostBuilder WithCommandFramework<T>(this IHostBuilder hostBuilder, Action<HostBuilderContext, ManagerBuilderContext> action = null)
            where T : CommandManager
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                var fxContext = new ManagerBuilderContext();

                action?.Invoke(hostContext, fxContext);

                services.AddCommandManager<T>(fxContext);
            });

            return hostBuilder;
        }
    }
}
