using ConfigServer.FileProvider;
using ConfigServer.TextProvider.Core;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class FileConfigRepositoryTests : IDisposable
    {
        private readonly IConfigRepository target;
        private readonly IConfigClientRepository clientTarget;

        private readonly string testdirectory;
        private readonly ConfigurationClient client; 
        private readonly ConfigurationIdentity configId;


        public FileConfigRepositoryTests()
        {
            testdirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/TestOutput/{Guid.NewGuid()}";
            var option = new FileConfigRespositoryBuilderOptions { ConfigStorePath = testdirectory };
            target = new TextStorageConfigurationRepository(new FileStorageConnector(option));
            clientTarget = new TextStorageConfigurationClientRepository(new FileStorageConnector(option));
            client = new ConfigurationClient
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4",
                Name = "Client 1",
                Description = "A description Client"
            };
            configId = new ConfigurationIdentity(client, new Version(1, 0));
            clientTarget.UpdateClientAsync(client).Wait();
        }

        public void Dispose()
        {
            var di = new DirectoryInfo(testdirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            Directory.Delete(testdirectory);
        }

        [Fact]
        public async Task CanSaveAndRetriveAsync()
        {
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId);


            await target.UpdateConfigAsync(config);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithTypeAsync()
        {
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = testValue }, configId);


            await target.UpdateConfigAsync(config);
            var result = (ConfigInstance<SimpleConfig>)(await target.GetAsync(typeof(SimpleConfig), configId));
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndGetClientsAsync()
        {
            await clientTarget.UpdateClientAsync(client);
            var result = (await clientTarget.GetClientsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(client.Name, result[0].Name);
            Assert.Equal(client.Description, result[0].Description);

        }

        [Fact]
        public async Task CanUpdateClientAsync()
        {
            var clientUpdated = new ConfigurationClient { ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4", Name = "Client 1", Description = "Client Description" };
            await clientTarget.UpdateClientAsync(client);
            await clientTarget.UpdateClientAsync(clientUpdated);

            var result = (await clientTarget.GetClientsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(clientUpdated.Name, result[0].Name);
            Assert.Equal(clientUpdated.Description, result[0].Description);
        }

        [Fact]
        public async Task Get_ReturnsNewObjectIfNotPresentAsync()
        {
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
            Assert.True(result.Any(a => a.IntProperty == testValue));
            Assert.True(result.Any(a => a.IntProperty == testValue2));


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
            var result = (IEnumerable<SimpleConfig>)(await target.GetCollectionAsync(typeof(SimpleConfig), configId));
            Assert.Equal(2, result.Count());
            Assert.True(result.Any(a => a.IntProperty == testValue));
            Assert.True(result.Any(a => a.IntProperty == testValue2));
        }
    }
}
