using ConfigServer.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Infrastructure
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
