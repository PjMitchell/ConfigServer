using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    public class ConfigServerClientBuilder
    {
        public ConfigServerClientBuilder(IServiceCollection serviceCollection, ConfigurationCollection configurationCollection)
        {
            ServiceCollection = serviceCollection;
            ConfigurationCollection = configurationCollection;
            ServiceCollection.AddSingleton(configurationCollection);
        }

        public IServiceCollection ServiceCollection { get; }

        public ConfigurationCollection ConfigurationCollection { get; }
    }
}
