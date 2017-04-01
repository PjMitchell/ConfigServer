using ConfigServer.InMemoryProvider;
using ConfigServer.Server;
using Microsoft.Extensions.Caching.Memory;
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
        readonly Mock<IConfigurationClientService> clientservice;
        readonly Mock<IResourceStore> resourceStore;

        readonly ConfigurationIdentity configIdentity = new ConfigurationIdentity(new ConfigurationClient("3E37AC18-A00F-47A5-B84E-C79E0823F6D4"));
        private readonly Uri testUri = new Uri("https://localhost:30300/");

        public LocalConfigServerTests()
        {
            var configurationCollection = new ConfigurationRegistry();
            configurationCollection.AddRegistration(ConfigurationRegistration.Build<SimpleConfig>());
            var repo = new InMemoryRepository();
            repository = repo;
            clientservice = new Mock<IConfigurationClientService>();
            clientservice.Setup(service => service.GetClientOrDefault(configIdentity.Client.ClientId))
                .ReturnsAsync(configIdentity.Client);
            resourceStore = new Mock<IResourceStore>();
        }

        [Fact]
        public async Task CanGetConfig()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected }, configIdentity));
            var localServer = new LocalConfigServerClient(repository, clientservice.Object, resourceStore.Object,configIdentity.Client.ClientId, testUri);
            var config =await localServer.GetConfigAsync<SimpleConfig>();
            Assert.Equal(expected, config.IntProperty);
        }

        [Fact]
        public async Task CanGetConfig_ByType()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected },configIdentity));
            var localServer = new LocalConfigServerClient(repository, clientservice.Object, resourceStore.Object, configIdentity.Client.ClientId, testUri);
            var config = await localServer.GetConfigAsync(typeof(SimpleConfig));
            var castedConfig = (SimpleConfig)config;
            Assert.Equal(expected, castedConfig.IntProperty);
        }
    }
}
