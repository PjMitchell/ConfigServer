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
        private readonly ConfigurationIdentity configId;


        public FileConfigRepositoryTests()
        {
            testdirectory = $"{AppContext.BaseDirectory}/TestOutput/{Guid.NewGuid()}";
            target = new FileConfigRepository(testdirectory);
            configId = new ConfigurationIdentity
            {
                ClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            target.CreateClientAsync(configId.ClientId).Wait();
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
            var config = new Config<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.SaveChangesAsync(config);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithTypeAsync()
        {
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.SaveChangesAsync(config);
            var result = (Config<SimpleConfig>)(await target.GetAsync(typeof(SimpleConfig), configId));
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanGetConfigSetIdsAsync()
        {
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.SaveChangesAsync(config);
            var result = (await target.GetClientIdsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configId.ClientId, result[0]);
        }

        [Fact]
        public async Task Get_ReturnsNewObjectIfNotPresentAsync()
        {
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ClientId = configId.ClientId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(configId.ClientId, config.ClientId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }


    }
}
