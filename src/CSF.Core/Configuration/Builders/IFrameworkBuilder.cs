using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public interface IFrameworkBuilder
    {
        /// <summary>
        ///     Gets or sets the pipeline service that will populate the framework.
        /// </summary>
        IPipelineService PipelineService { get; set; }

        /// <summary>
        ///     Gets or sets the services used to configure the framework.
        /// </summary>
        IServiceProvider Services { get; set; }

        /// <summary>
        ///     Gets or sets the configuration used to configure the framework.
        /// </summary>
        CommandConfiguration Configuration { get; set; }

        /// <summary>
        ///     Modifies the <see cref="Configuration"/> of this builder.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The current <see cref="FrameworkBuilder{T}"/> for chaining calls.</returns>
        IFrameworkBuilder ConfigureCommands(Action<CommandConfiguration> action);

        /// <summary>
        ///     Sets the <see cref="Services"/> of this builder.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The current <see cref="FrameworkBuilder{T}"/> for chaining calls.</returns>
        IFrameworkBuilder ConfigureServices(IServiceProvider services);

        /// <summary>
        ///     Builds the current builder into a new <see cref="CommandFramework{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="CommandFramework{T}"/> with provided builder configuration.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        ICommandFramework Build();
    }
}
