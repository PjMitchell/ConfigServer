using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    public class ConfigServerBuilder
    {
        public ConfigServerBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            ConfigurationSetCollection = new ConfigurationSetCollection();
            ServiceCollection.AddSingleton(ConfigurationSetCollection);
        }

        public IServiceCollection ServiceCollection { get; }

        public ConfigurationSetCollection ConfigurationSetCollection { get; }

    }
}
