using ConfigServer.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Infrastructure
{
    public class ConfigServerBuilder
    {
        public ConfigServerBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            ConfigurationSetCollection = new ConfigurationSetCollection();
            ServiceCollection.AddInstance(ConfigurationSetCollection);
        }

        public IServiceCollection ServiceCollection { get; }

        public ConfigurationSetCollection ConfigurationSetCollection { get; }

    }
}
