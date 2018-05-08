using ConfigServer.InMemoryProvider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class InMemoryRespositoryTests
    {
        private readonly ConfigurationRegistry configurationCollection;
        private readonly IConfigRepository target;
        private readonly IConfigClientRepository clientTarget;


        public InMemoryRespositoryTests()
        {
            configurationCollection = new ConfigurationRegistry();
            configurationCollection.BuildAndAddRegistration<SimpleConfig>();
            var repo = new InMemoryRepository();
            target = repo;
            clientTarget = repo;
        }

        [Fact]
        public async Task CanSaveAndRetriveAsync()
        {
            var configId = new ConfigurationIdentity(new ConfigurationClient("3E37AC18-A00F-47A5-B84E-C79E0823F6D4"), new Version(1, 0));
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId);


            await target.UpdateConfigAsync(config);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithTypeAsync()
        {
            var configId = new ConfigurationIdentity(new ConfigurationClient("3E37AC18-A00F-47A5-B84E-C79E0823F6D4"), new Version(1, 0));
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId);


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
            await clientTarget.UpdateClientAsync(client);
            var result =(await clientTarget.GetClientsAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(client.Name, result[0].Name);
            Assert.Equal(client.Description, result[0].Description);

        }

        [Fact]
        public async Task CanUpdateClientAsync()
        {
            var client = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4", Name = "Cleint 1", Description = "Cleint Description" };
            var clientUpdated = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4", Name = "Client 1", Description = "Client Description" };
            await clientTarget.UpdateClientAsync(client);
            await clientTarget.UpdateClientAsync(clientUpdated);

            var result = (await clientTarget.GetClientsAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(clientUpdated.Name, result[0].Name);
            Assert.Equal(clientUpdated.Description, result[0].Description);
        }

        [Fact]
        public async Task Get_ReturnsNewObjectIfNotPresentAsync()
        {
            var client = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4" };

            var configId = new ConfigurationIdentity(client, new Version(1, 0));
            const int testValue = 23;

            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId);

            await clientTarget.UpdateClientAsync(client);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(client.ClientId, config.ConfigurationIdentity.Client.ClientId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveCollectionAsync()
        {
            var configId = new ConfigurationIdentity(new ConfigurationClient("3E37AC18-A00F-47A5-B84E-C79E0823F6D4"), new Version(1, 0));
            const int testValue = 23;
            const int testValue2 = 24;
            var values = new[]
            {
                new SimpleConfig { IntProperty = testValue },
                new SimpleConfig { IntProperty = testValue2 }
            };
            var config = new ConfigCollectionInstance<SimpleConfig>(values, configId);


            await target.UpdateConfigAsync(config);
            var result = await target.GetCollectionAsync<SimpleConfig>(configId);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.IntProperty == testValue);
            Assert.Contains(result, a => a.IntProperty == testValue2);


        }

        [Fact]
        public async Task CanSaveAndRetriveCollectionWithTypeAsync()
        {
            var configId = new ConfigurationIdentity(new ConfigurationClient("3E37AC18-A00F-47A5-B84E-C79E0823F6D4"), new Version(1, 0));
            const int testValue = 23;
            const int testValue2 = 24;
            var values = new[]
            {
                new SimpleConfig { IntProperty = testValue },
                new SimpleConfig { IntProperty = testValue2 }
            };
            var config = new ConfigCollectionInstance<SimpleConfig>(values, configId);


            await target.UpdateConfigAsync(config);
            var result = (IEnumerable<SimpleConfig>)(await target.GetCollectionAsync(typeof(SimpleConfig),configId));
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.IntProperty == testValue);
            Assert.Contains(result, a => a.IntProperty == testValue2);
        }
    }
}
