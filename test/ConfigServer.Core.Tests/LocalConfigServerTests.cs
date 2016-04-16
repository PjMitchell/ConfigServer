using ConfigServer.InMemoryProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class LocalConfigServerTests
    {
        readonly IConfigRepository repository;
        const string configSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";

        public LocalConfigServerTests()
        {
            var configurationCollection = new ConfigurationCollection();
            configurationCollection.AddRegistration(ConfigurationRegistration.Build<SimpleConfig>());
            repository = new InMemoryRepository(configurationCollection);
            
        }

        [Fact]
        public void CanGetConfig()
        {
            var expected = 23;
            repository.SaveChanges(new Config<SimpleConfig> { ConfigSetId = configSetId, Configuration = new SimpleConfig { IntProperty = expected } });
            var localServer = new LocalConfigServer(repository,configSetId);
            var config = localServer.BuildConfig<SimpleConfig>();
            Assert.Equal(expected, config.IntProperty);
        }

        [Fact]
        public void CanGetConfig_ByType()
        {
            var expected = 23;
            repository.SaveChanges(new Config<SimpleConfig> { ConfigSetId = configSetId, Configuration = new SimpleConfig { IntProperty = expected } });
            var localServer = new LocalConfigServer(repository, configSetId);
            var config = (SimpleConfig)localServer.BuildConfig(typeof(SimpleConfig));
            Assert.Equal(expected, config.IntProperty);
        }
    }
}
