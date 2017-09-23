using ConfigServer.InMemoryProvider;
using ConfigServer.Server;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class LocalConfigServerTests
    {
        readonly IConfigRepository repository;
        readonly Mock<IConfigurationClientService> clientservice;
        readonly Mock<IResourceStore> resourceStore;
        readonly Mock<IConfigurationModelRegistry> registry;
        const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        readonly ConfigurationIdentity configIdentity = new ConfigurationIdentity(new ConfigurationClient(clientId), new Version(1, 0));
        private readonly LocalServerClientOptions options = new LocalServerClientOptions(clientId, new Uri("https://localhost:30300/"));

        public LocalConfigServerTests()
        {
            var configurationCollection = new ConfigurationRegistry();
            configurationCollection.AddRegistration(ConfigurationRegistration.Build<SimpleConfig>());
            var repo = new InMemoryRepository();
            repository = repo;
            clientservice = new Mock<IConfigurationClientService>();
            clientservice.Setup(service => service.GetClientOrDefault(configIdentity.Client.ClientId))
                .ReturnsAsync(configIdentity.Client);
            registry = new Mock<IConfigurationModelRegistry>();
            registry.Setup(s => s.GetVersion())
                .Returns(() => configIdentity.ServerVersion);
            resourceStore = new Mock<IResourceStore>();
        }

        [Fact]
        public async Task CanGetConfig()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected }, configIdentity));
            var localServer = new LocalConfigServerClient(repository, clientservice.Object, registry.Object, options);
            var config =await localServer.GetConfigAsync<SimpleConfig>();
            Assert.Equal(expected, config.IntProperty);
        }

        [Fact]
        public async Task CanGetConfig_ByType()
        {
            var expected = 23;
            await repository.UpdateConfigAsync(new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = expected },configIdentity));
            var localServer = new LocalConfigServerClient(repository, clientservice.Object, registry.Object, options);
            var config = await localServer.GetConfigAsync(typeof(SimpleConfig));
            var castedConfig = (SimpleConfig)config;
            Assert.Equal(expected, castedConfig.IntProperty);
        }
    }
}
