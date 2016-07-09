using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    /// <summary>
    /// ConfigServer client builder.
    /// Used in the initial configuration of the ConfigServer client
    /// </summary>
    public class ConfigServerClientBuilder
    {

        internal ConfigServerClientBuilder(IServiceCollection serviceCollection, ConfigurationRegistry configurationCollection)
        {
            ServiceCollection = serviceCollection;
            ConfigurationRegistry = configurationCollection;
            ServiceCollection.AddSingleton(configurationCollection);
        }

        /// <summary>
        /// ServiceCollection for builder
        /// </summary>
        public IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// ConfigurationRegistry for builder that forms a registry of available configurations types 
        /// </summary>
        public ConfigurationRegistry ConfigurationRegistry { get; }
    }
}
