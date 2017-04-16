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
            var registry = new ConfigurationSetRegistry();
            ConfigurationSetCollection = registry;
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
        public IConfigurationSetRegistry ConfigurationSetCollection { get; }

    }
}
