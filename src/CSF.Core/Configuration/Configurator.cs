using System;
using System.Collections.Generic;
using System.Text;

namespace CSF
{
    public abstract class Configurator
    {
        public IServiceProvider Provider { get; }

        /// <summary>
        ///     The configuration that will eventually end up
        /// </summary>
        public CommandConfiguration Configuration { get; }

        public Configurator(IServiceProvider provider, CommandConfiguration configuration)
        {
            Provider = provider;
            Configuration = configuration;
        }

        public virtual TypeReaderProvider ConfigureTypeReaders()
        {
            var dict = new TypeReaderProvider(TypeReader.CreateDefaultReaders());

            foreach (var reader in AutonomousTypeReaderRegistration())
                dict.Include(reader.Type, reader);

            return dict;
        }

        public virtual List<ITypeReader> AutonomousTypeReaderRegistration()
        {
            var list = new List<ITypeReader>();

            var tt = typeof(ITypeReader);

            foreach (var assembly in Configuration.RegistrationAssemblies)
                foreach (var type in assembly.ExportedTypes)
                    if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        list.Add(BuildTypeReader(type));

            return list;
        }

        public virtual ITypeReader BuildTypeReader(Type type)
        {
            var reader = TypeReaderInfo.Build(type);

            var output = reader.Construct(Provider);

            if (output is null)
                throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (output is ITypeReader typeReader)
                return typeReader;

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(ITypeReader)}.");
        }

        public virtual ResultHandlerProvider ConfigureResultHandlers()
        {
            var dict = new ResultHandlerProvider();

            foreach (var result in AutonomousResultHandlerRegistration())
                dict.Include(result);

            return dict;
        }

        public virtual List<IResultHandler> AutonomousResultHandlerRegistration()
        {
            var list = new List<IResultHandler>();

            var tt = typeof(IResultHandler);

            foreach (var assembly in Configuration.RegistrationAssemblies)
                foreach (var type in assembly.ExportedTypes)
                    if (tt.IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic)
                        list.Add(BuildResultHandler(type));

            return list;
        }

        public virtual IResultHandler BuildResultHandler(Type type)
        {
            var handler = ResultHandlerInfo.Build(type);

            var output = handler.Construct(Provider);

            if (output is null)
                throw new ArgumentNullException(nameof(type), "The provided type caused an invalid return type.");

            if (output is IResultHandler resultHandler)
                return resultHandler;

            throw new InvalidOperationException($"Could not box {type.Name} as {nameof(IResultHandler)}.");
        }

        public virtual PrefixProvider ConfigurePrefixes()
        {
            var dict = new PrefixProvider();

            foreach (var prefix in AutonomousPrefixRegistration())
                dict.Include(prefix);

            return dict;
        }

        public virtual List<IPrefix> AutonomousPrefixRegistration()
        {
            var list = new List<IPrefix>();

            return list;
        }

        public virtual IPrefix BuildPrefix(Type type)
        {
            return null;
        }

        /// <summary>
        ///     Configures the application logger and exposes it within the pipeline.
        /// </summary>
        /// <remarks>
        ///     This method can be overridden to modify what will be registered.
        /// </remarks>
        /// <returns></returns>
        public virtual ILogger ConfigureLogger()
        {
            return new DefaultLogger(Configuration.DefaultLogLevel);
        }
    }
}
