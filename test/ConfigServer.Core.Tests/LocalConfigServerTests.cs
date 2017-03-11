using ConfigServer.InMemoryProvider;
using ConfigServer.Server;
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
            var configurationCollection = new ConfigurationRegistry();
            configurationCollection.AddRegistration(ConfigurationRegistration.Build<SimpleConfig>());
            repository = new InMemoryRepository();
            
        }

        [Fact]
        public async Task CanGetConfig()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected }, configSetId));
            var localServer = new LocalConfigServerClient(repository,configSetId);
            var config =await localServer.GetConfigAsync<SimpleConfig>();
            Assert.Equal(expected, config.IntProperty);
        }

        [Fact]
        public async Task CanGetConfig_ByType()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected },configSetId));
            var localServer = new LocalConfigServerClient(repository, configSetId);
            var config = await localServer.GetConfigAsync(typeof(SimpleConfig));
            var castedConfig = (SimpleConfig)config;
            Assert.Equal(expected, castedConfig.IntProperty);
        }
    }
}
