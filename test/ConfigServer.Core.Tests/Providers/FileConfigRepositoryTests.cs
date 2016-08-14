using ConfigServer.FileProvider;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class FileConfigRepositoryTests : IDisposable
    {
        private readonly IConfigRepository target;
        private readonly string testdirectory;
        private readonly ConfigurationClient client; 
        private readonly ConfigurationIdentity configId;


        public FileConfigRepositoryTests()
        {
            testdirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/TestOutput/{Guid.NewGuid()}";
            target = new FileConfigRepository(new FileConfigRespositoryBuilderOptions {  ConfigStorePath = testdirectory });
            client = new ConfigurationClient
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4",
                Name = "Client 1",
                Description = "A description Client"
            };
            configId = new ConfigurationIdentity
            {
                ClientId = client.ClientId
            };
            target.UpdateClientAsync(client).Wait();
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
            var config = new ConfigInstance<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.UpdateConfigAsync(config);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithTypeAsync()
        {
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.UpdateConfigAsync(config);
            var result = (ConfigInstance<SimpleConfig>)(await target.GetAsync(typeof(SimpleConfig), configId));
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndGetClientsAsync()
        {
            await target.UpdateClientAsync(client);
            var result = (await target.GetClientsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(client.ClientId, result[0].ClientId);
            Assert.Equal(client.Name, result[0].Name);
            Assert.Equal(client.Description, result[0].Description);

        }

        [Fact]
        public async Task CanUpdateClientAsync()
        {
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
            const int testValue = 23;
            var config = new ConfigInstance<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.UpdateClientAsync(client);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(client.ClientId, config.ClientId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }

    }
}
