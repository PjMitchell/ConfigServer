using ConfigServer.InMemoryProvider;
using ConfigServer.Server;
using Moq;
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
        readonly Mock<IResourceStore> resourceStore;

        const string configSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private readonly Uri testUri = new Uri("https://localhost:30300/");

        public LocalConfigServerTests()
        {
            var configurationCollection = new ConfigurationRegistry();
            configurationCollection.AddRegistration(ConfigurationRegistration.Build<SimpleConfig>());
            repository = new InMemoryRepository();
            resourceStore = new Mock<IResourceStore>();
        }

        [Fact]
        public async Task CanGetConfig()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected }, configSetId));
            var localServer = new LocalConfigServerClient(repository, resourceStore.Object,configSetId, testUri);
            var config =await localServer.GetConfigAsync<SimpleConfig>();
            Assert.Equal(expected, config.IntProperty);
        }

        [Fact]
        public async Task CanGetConfig_ByType()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected },configSetId));
            var localServer = new LocalConfigServerClient(repository, resourceStore.Object, configSetId, testUri);
            var config = await localServer.GetConfigAsync(typeof(SimpleConfig));
            var castedConfig = (SimpleConfig)config;
            Assert.Equal(expected, castedConfig.IntProperty);
        }
    }
}
