using ConfigServer.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Server
{
    /// <summary>
    /// ConfigServer builder.
    /// Used in the initial configuration of ConfigServer
    /// </summary>
    public class ConfigServerBuilder
    {
        internal ConfigServerBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            ConfigurationSetCollection = new ConfigurationSetRegistry();
            ServiceCollection.AddSingleton(ConfigurationSetCollection);
        }

        /// <summary>
        /// ServiceCollection for builder
        /// </summary>
        public IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// ConfigurationSetRegistry for builder that forms a registry of available configurations sets for the server 
        /// </summary>
        public ConfigurationSetRegistry ConfigurationSetCollection { get; }

    }
}
