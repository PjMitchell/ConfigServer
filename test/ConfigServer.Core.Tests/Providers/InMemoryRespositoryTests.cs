using ConfigServer.InMemoryProvider;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class InMemoryRespositoryTests
    {
        private readonly ConfigurationCollection configurationCollection;
        private readonly IConfigRepository target;

        public InMemoryRespositoryTests()
        {
            configurationCollection = new ConfigurationCollection();
            configurationCollection.BuildAndAddRegistration<SimpleConfig>();
            target = new InMemoryRepository();
        }



        [Fact]
        public async Task CanSaveAndRetriveAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            
            await target.SaveChangesAsync(config);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithTypeAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.SaveChangesAsync(config);
            var result = (Config<SimpleConfig>)(await target.GetAsync(typeof(SimpleConfig),configId));
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanGetConfigSetIdsAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.SaveChangesAsync(config);
            var result =(await target.GetConfigSetIdsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configId.ConfigSetId, result[0]);
        }

        [Fact]
        public async Task CanCreateConfigSetIdsAsync()
        {
            var configSet = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";

            await target.CreateConfigSetAsync(configSet);
            var result = (await target.GetConfigSetIdsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configSet, result[0]);
        }

        [Fact]
        public async Task Get_ReturnsNewObjectIfNotPresentAsync()
        {
            var configSet = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = configSet
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };

            await target.CreateConfigSetAsync(configSet);
            var result = await target.GetAsync<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(configSet, config.ConfigSetId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }
    }
}
