using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSF
{
    public sealed class FrameworkBuilder<T> : IFrameworkBuilder
        where T : PipelineService
    {
        /// <summary>
        ///     Gets or sets the pipeline service that will populate the framework.
        /// </summary>
        public T PipelineService { get; set; }

        /// <inheritdoc/>
        public IServiceProvider Services { get; set; }

        /// <inheritdoc/>
        public CommandConfiguration Configuration { get; set; }

        /// <inheritdoc/>
        public IHandlerBuilder HandlerBuilder { get; set; }

        /// <inheritdoc/>
        IPipelineService IFrameworkBuilder.PipelineService { get; set; }

        /// <summary>
        ///     Creates a new <see cref="FrameworkBuilder{T}"/>.
        /// </summary>
        internal FrameworkBuilder(T pipelineService)
        {
            PipelineService = pipelineService;
            Services = EmptyServiceProvider.Instance;
            Configuration = new CommandConfiguration();
            HandlerBuilder = new HandlerBuilder();
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureCommands(Action<CommandConfiguration> action)
        {
            action(Configuration);
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureServices(IServiceProvider services)
        {
            Services = services;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkBuilder ConfigureHandlers(Action<IHandlerBuilder> action)
        {
            action(HandlerBuilder);
            return this;
        }

        /// <inheritdoc/>
        public ICommandFramework Build()
        {
            if (!HandlerBuilder.Cast<HandlerBuilder>().IsUselessToBuild)
            {
                var handler = HandlerBuilder.Build();

                Configuration.ResultHandlers.Include(handler);
            }

            if (PipelineService is null)
                throw new ArgumentNullException(nameof(PipelineService));

            return new CommandFramework<T>(Services, Configuration, PipelineService);
        }

        /// <inheritdoc/>
        public async Task BuildAndRunAsync(CancellationToken cancellationToken = default)
            => await Build().RunAsync();
    }
}
