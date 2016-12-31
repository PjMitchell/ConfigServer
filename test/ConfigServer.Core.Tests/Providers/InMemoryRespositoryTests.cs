using ConfigServer.InMemoryProvider;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class InMemoryRespositoryTests
    {
        private readonly ConfigurationRegistry configurationCollection;
        private readonly IConfigRepository target;

        public InMemoryRespositoryTests()
        {
            configurationCollection = new ConfigurationRegistry();
            configurationCollection.BuildAndAddRegistration<SimpleConfig>();
            target = new InMemoryRepository();
        }

        [Fact]
        public async Task CanSaveAndRetriveAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId.ClientId);


            await target.UpdateConfigAsync(config);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithTypeAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId.ClientId);


            await target.UpdateConfigAsync(config);
            var result = (ConfigInstance<SimpleConfig>)(await target.GetAsync(typeof(SimpleConfig),configId));
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndGetClientsAsync()
        {
            var client = new ConfigurationClient
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4",
                Name = "Client 1",
                Description = "A description Client"
            };
            await target.UpdateClientAsync(client);
            var result =(await target.GetClientsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(client.Name, result[0].Name);
            Assert.Equal(client.Description, result[0].Description);

        }

        [Fact]
        public async Task CanUpdateClientAsync()
        {
            var client = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4", Name = "Cleint 1", Description = "Cleint Description" };
            var clientUpdated = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4", Name = "Client 1", Description = "Client Description" };
            await target.UpdateClientAsync(client);
            await target.UpdateClientAsync(clientUpdated);

            var result = (await target.GetClientsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(clientUpdated.Name, result[0].Name);
            Assert.Equal(clientUpdated.Description, result[0].Description);
        }

        [Fact]
        public async Task Get_ReturnsNewObjectIfNotPresentAsync()
        {
            var client = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4" };

            var configId = new ConfigurationIdentity
            {
                ClientId = client.ClientId
            };
            const int testValue = 23;

            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId.ClientId);

            await target.UpdateClientAsync(client);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(client.ClientId, config.ClientId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }
    }
}
