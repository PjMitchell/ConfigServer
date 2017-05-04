using ConfigServer.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// ConfigServer builder.
    /// Used in the initial configuration of ConfigServer
    /// </summary>
    public class ConfigServerBuilder
    {
        private readonly ConfigurationSetRegistry registry;

        internal ConfigServerBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            registry = new ConfigurationSetRegistry();
            ServiceCollection.AddSingleton(registry);
            ServiceCollection.AddSingleton<IConfigurationSetRegistry>(registry);
        }

        /// <summary>
        /// ServiceCollection for builder
        /// </summary>
        public IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// ConfigurationSetRegistry for builder that forms a registry of available configurations sets for the server 
        /// </summary>
        public IConfigurationSetRegistry ConfigurationSetCollection => registry;

        /// <summary>
        /// Adds new configuration set to the registry
        /// </summary>
        /// <param name="model">ConfigurationSetModel to be added to the registry</param>
        /// <returns>returns true if successful or false if registry already contains configuration set type</returns>
        public bool AddConfigurationSet(ConfigurationSetModel model) => registry.AddConfigurationSet(model);

        /// <summary>
        /// Sets version for configurations
        /// </summary>
        /// <param name="version">Configuration version</param>
        public void SetVersion(Version version) => registry.SetVersion(version);

    }
}
