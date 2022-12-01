using System;
using System.Collections.Generic;
using System.Text;
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

        /// <summary>
        ///     Gets or sets the services used to configure the framework.
        /// </summary>
        public IServiceProvider Services { get; set; }

        /// <summary>
        ///     Gets or sets the configuration used to configure the framework.
        /// </summary>
        public CommandConfiguration Configuration { get; set; }

        IPipelineService IFrameworkBuilder.PipelineService { get; set; }

        /// <summary>
        ///     Creates a new <see cref="FrameworkBuilder{T}"/>.
        /// </summary>
        internal FrameworkBuilder(T pipelineService)
        {
            PipelineService = pipelineService;
            Services = EmptyServiceProvider.Instance;
            Configuration = new CommandConfiguration();
        }

        /// <summary>
        ///     Modifies the <see cref="Configuration"/> of this builder.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The current <see cref="FrameworkBuilder{T}"/> for chaining calls.</returns>
        public IFrameworkBuilder ConfigureCommands(Action<CommandConfiguration> action)
        {
            action(Configuration);

            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Services"/> of this builder.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The current <see cref="FrameworkBuilder{T}"/> for chaining calls.</returns>
        public IFrameworkBuilder ConfigureServices(IServiceProvider services)
        {
            Services = services;

            return this;
        }

        /// <summary>
        ///     Builds the current builder into a new <see cref="CommandFramework{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="CommandFramework{T}"/> with provided builder configuration.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ICommandFramework Build()
        {
            if (PipelineService is null)
                throw new ArgumentNullException(nameof(PipelineService));

            return new CommandFramework<T>(Services, Configuration, PipelineService);
        }
    }

    public static class FrameworkBuilder
    {
        /// <summary>
        ///     Creates a new <see cref="FrameworkBuilder{T}"/> from the provided pipeline setup.
        /// </summary>
        /// <param name="pipelineService"></param>
        /// <returns></returns>
        public static IFrameworkBuilder CreateDefaultBuilder<TService>(TService pipelineService)
            where TService : PipelineService
        {
            return new FrameworkBuilder<TService>(pipelineService);
        }

        /// <summary>
        ///     Creates a new <see cref="FrameworkBuilder{T}"/> with default setup.
        /// </summary>
        /// <returns></returns>
        public static IFrameworkBuilder CreateDefaultBuilder()
        {
            return new FrameworkBuilder<PipelineService>(new PipelineService());
        }

        /// <summary>
        ///     Creates a new <see cref="CommandFramework{T}"/> with default setup directly. It skips all configuration.
        /// </summary>
        /// <returns></returns>
        public static ICommandFramework BuildWithMinimalSetup()
        {
            return CreateDefaultBuilder()
                .Build();
        }
    }
}
